using UnityEngine;

//Author: William Rapprich
//Last edited: 29.11.2017 by William
public class PressurePlate : Activator
{
	enum Weight
    {
        light = 1,
        heavy = 2
    }
	
	[SerializeField] Weight requiredWeight = Weight.light;
	[SerializeField] float requiredCenterDistance = 0.3f;
	Collider activatingCollider = null;

	void OnTriggerStay(Collider other)
	{
		MoveableBlock block = null;
		if (other.transform.parent)
			block = other.transform.parent.GetComponent<MoveableBlock>();

		if ( !IsActive && 
				( 
					(
						requiredWeight == Weight.heavy
						&& block != null && (Weight)block.Mass == requiredWeight
						&& (other.transform.position - transform.position).magnitude <= requiredCenterDistance
					)  
					||
					(
						requiredWeight == Weight.light 
						&& (other.CompareTag("Player") || other.CompareTag("Pet") || block != null) 
						&& (other.transform.position - transform.position).magnitude <= requiredCenterDistance 
					) 
				)
			)
		{
			activatingCollider = other;
			IsActive = true;
			anim.SetBool("isActive", true);
			stateChange.Invoke(true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (IsActive && other == activatingCollider)
		{
			activatingCollider = null;
			IsActive = false;
			anim.SetBool("isActive", false);
			stateChange.Invoke(false);
		}
	}
}
