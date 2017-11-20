using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

//Author: Johannes Krause 
//Last edited: 15.11.2017 by: William

public abstract class Enemy : MonoBehaviour, IHitable
{
    [Header("Health", order = 0)]
    [SerializeField]
    protected int startHp;
    protected int hp; //current health
    [SerializeField] [Range(-20f, 20f)] float healthBarOffset = 3.5f; //y offset from enemy position
    [HideInInspector] public EnemyHealthChangeEvent healthChange;
	private string id;

    [Header("Movement", order = 1)]
    [SerializeField]
    protected float speed;
    [SerializeField] protected float aggroRadius;


    [Header("Attacking", order = 2)]
    [SerializeField]
    protected float hitRange;
    [SerializeField] protected int damage;
    [SerializeField] protected float hitDelay;
    protected float timeSinceLastHit = 0;

    [Header("Bounding Sphere", order = 3)]
    [SerializeField]
    float boundingSphereRadius = 2f; // radius of the corresponding bounding sphere used for culling

    protected NavMeshAgent navAgent;
    protected bool sleeping;
    private Transform target;
    protected IHitable hitTarget;

    public virtual Transform Target
    {
        get { return target; }
        set { target = value; }
    }

    private void Awake()
    {
		healthChange = new EnemyHealthChangeEvent();
        hp = startHp;

        // Make health bar canvas if there isn't one already
        if(EnemyHealthDisplay.Instance == null)
        {
            Instantiate(PrefabHolder.Instance.enemyHealthBarCanvas);
        }

		id = System.Guid.NewGuid ().ToString ();

        EnemyPool.Instance.Register(this); //Register for pooling
    }

    public void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = 0;
        navAgent.acceleration = speed * 10;
        sleeping = true;
    }

    public void Update()
    {
        GeneralRoutine();
        if (sleeping && TargetInRange(aggroRadius))
        {
            WakingUp();
        }

        if (!sleeping && TargetInRange(hitRange))
        {
            if (timeSinceLastHit <= 0)
                AttackTarget();
        }
    }

	public string getId(){
		return this.id;
	}

    protected virtual void GeneralRoutine()
    {
        if (timeSinceLastHit > 0)
            timeSinceLastHit -= Time.deltaTime;
        navAgent.destination = GetMovingPosition();
    }

    public virtual void Hit(HitInfo info)
    {
        hp -= info.damage;
        healthChange.Invoke(this, hp, startHp); //trigger event for all listeners
        if (sleeping && (info.source.tag.Equals("Player") || info.source.tag.Equals("Pet"))) WakingUp();
        
        if (hp <= 0)
        {
            Dies();
        }
    }

    protected virtual bool TargetInRange(float range)
    {
        float playerDistance = Vector3.Distance(target.position, transform.position);
        return (playerDistance < range);
    }
    
    protected virtual void AttackTarget()
    {
        hitTarget.Hit(new HitInfo(this.gameObject, this.damage, this.transform.forward));
        timeSinceLastHit = hitDelay;
    }

    protected virtual void Dies()
    {
        EnemyHealthDisplay.Instance.Remove(this); //hide health bar
        EnemyPool.Instance.Remove(this); //Remove from pooling
        Destroy(gameObject);
    }

    private void WakingUp()
    {
        navAgent.speed = speed;
        sleeping = false;
    }

    /// <summary>
    /// Called by culling group when an enemy enters view frustum.
    /// </summary>
    public virtual void Show()
    {
        //show health bar
        EnemyHealthDisplay.Instance.Register(this, hp, startHp);
    }

    /// <summary>
    /// Called by culling group when an enemy leaves view frustum.
    /// </summary>
    public virtual void Cull()
    {
        //hide health bar
        EnemyHealthDisplay.Instance.Remove(this);
    }

    public float GetBoundingSphereRadius()
    {
        return boundingSphereRadius;
    }

        /// <returns>Center point for the floating health bar.</returns>
    public Vector3 GetHealthBarPos()
    {
        return transform.position + Vector3.up * healthBarOffset; //add offset
    }
    
    /// <summary>
    /// Method, that determines the position for the NavAgent for this enemy from which to attack.
    /// </summary>
    /// <returns></returns>
    protected virtual Vector3 GetMovingPosition()
    {
        return target.position;
    }
}

/// <summary>
/// Event to inform about health changes to an enemy, passes on current and starting health.
/// </summary>
public class EnemyHealthChangeEvent: UnityEvent<Enemy, int, int>
{} 