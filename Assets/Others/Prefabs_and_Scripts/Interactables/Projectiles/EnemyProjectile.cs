using UnityEngine;

//Author: Jannick Sickel
// last edited 20.11.17 by William

public class EnemyProjectile : ProjectileBasics
{

    void Start()
    {
        direction = transform.forward;
		Setup ();
    }

    void Update()
    {
        Move();
    }

    protected override void OnHit(RaycastHit hit)
    {
		IHitable hitable = hit.collider.gameObject.GetComponent<IHitable> ();

		if (hitable != null)
        {
			hitable.Hit (new HitInfo (gameObject, damage, direction));
		}

        Destroy(gameObject);
    }
}
