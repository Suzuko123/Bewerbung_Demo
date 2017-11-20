using UnityEngine;
using UnityEngine.Events;

// Author: William Rapprich
// Last modified: 20.11.2017 by William Rapprich

/// <summary>
/// Behaviour of the player character.
/// </summary>
public class Player : MonoBehaviour, IHitable
{
    /// <summary>
    /// Singleton Instance
    /// </summary>
    /// <returns>Singleton Instance</returns>
	public static Player Instance { get; private set; }

	[Header("Movement")]
	[SerializeField] float speed = 10;
	[HideInInspector] public float speedMultiplier = 1f;

    Vector3 velocity = Vector3.zero;
    public Vector2 MovementInput {get; set;}

    public bool IsSliding {get; set;}

    float fallSpeed = 0;
    [SerializeField] float fallMultiplier = 1.0f;

    CharacterController charCon;
    

    [Header("Combat")]
    [SerializeField] int maxHp = 8;
    [SerializeField] int hp = 8;

    [HideInInspector] public HealthChangeEvent healthChange;

    void Awake()
    {
		// Singleton
		if (Instance != null && Instance != this)
		{
			// Destroy the heretic if there is another instance
			Destroy(gameObject);
			return;
		}
		Instance = this;

		healthChange = new HealthChangeEvent();
    }

	private void Start()
	{
        charCon = GetComponent<CharacterController>();

        hp = maxHp;

        if (PlayerHealthDisplay.Instance == null)
			Instantiate(PrefabHolder.Instance.inGameUI);
    }

	void Update()
	{
        if (!IsSliding && charCon.isGrounded)
        {
            MovementInput = GetMovement();
            // Rotate player
            if (MovementInput != Vector2.zero)
                transform.localRotation = Quaternion.LookRotation(new Vector3(MovementInput.x, 0f, MovementInput.y));  
        }

        fallSpeed += fallMultiplier * Physics.gravity.y * Time.deltaTime;
		velocity = new Vector3(MovementInput.x * (speed*speedMultiplier), fallSpeed, MovementInput.y * (speed*speedMultiplier));
        
        if (charCon.isGrounded)
        {
            charCon.Move(velocity * Time.deltaTime + Vector3.up * -charCon.stepOffset);
			fallSpeed = -Mathf.Tan(charCon.slopeLimit * Mathf.Deg2Rad) * (speed*speedMultiplier);
        }
        else
        {
            charCon.Move(velocity * Time.deltaTime);
            IsSliding = false;
            MovementInput = Vector2.zero;
        }
	}

    /// <summary>
    /// Processes movement input.
    /// </summary>
    /// <returns>Vector2 movement direction</returns>
    public Vector2 GetMovement()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetButton("up")) movement += Vector2.up;
        if (Input.GetButton("down")) movement += Vector2.down;
        if (Input.GetButton("left")) movement += Vector2.left;
        if (Input.GetButton("right")) movement += Vector2.right;

        return movement;
    }

    /// <summary>
    /// Handles player being hit.
    /// </summary>
    public void Hit(HitInfo info)
    {
        hp = Mathf.Min(hp - info.damage, maxHp);
        healthChange.Invoke(hp);
		if (hp <= 0)
		{
			GameManager.Reset();
		}
	}

    /// <returns>Maximum player HP</returns>
    public int GetMaxHp()
    {
        return maxHp;
    }

    /// <returns>Current player HP</returns>
    public int GetHp()
    {
		return hp;
	}

    /// <returns>Player's character controller height</returns>
    public float GetHeight()
    {
        return charCon.height;
    }

    /// <returns>Player's character controller radius</returns>
    public float GetRadius()
    {
        return charCon.radius;
    }
}

/// <summary>
/// Informs listeners about player's current HP
/// </summary>
public class HealthChangeEvent : UnityEvent<int>
{} 