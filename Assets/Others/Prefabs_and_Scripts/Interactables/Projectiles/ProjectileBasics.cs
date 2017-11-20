using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Jannick Sickel
// last edited 20.11.2017 by William

public abstract class ProjectileBasics : MonoBehaviour
{
    [SerializeField] protected float velocity = 25f;
    [SerializeField] protected float range = 10f;
    [SerializeField] protected int damage = 1;
	[SerializeField] protected float radius = .2f;

	protected LayerMask collisionLayers;
    protected Vector3 direction;

	protected void Setup(){
		int myLayer = gameObject.layer;
		int layerMask = 0;
		for(int i = 0; i < 32;i++) {
			if(!Physics.GetIgnoreLayerCollision(myLayer, i))  {
				layerMask |= 1 << i;
			}
		}
		collisionLayers = layerMask;
	}

    protected void Move()
    {
        range -= velocity * Time.deltaTime;
        if (range <= 0)
        {
            Destroy(gameObject);
        }
			
		RaycastHit hit;

		if (Physics.SphereCast(transform.position, radius,  direction, out hit, velocity * Time.deltaTime, collisionLayers))
		{
			OnHit(hit);
		}

        transform.position += direction * velocity * Time.deltaTime;
    }

	abstract protected void OnHit(RaycastHit hit);

    public void SetAttributes(float velocity, float range, int damage)
    {
        this.velocity = velocity;
        this.range = range;
        this.damage = damage;
    }
}
