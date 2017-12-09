using UnityEngine;

//Author: William Rapprich
//Last edited: 29.11.2017 by William
public class SimpleActivator : Activator
{
    void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") || other.CompareTag("Pet"))
		{
			IsActive = true;
			anim.SetBool("isActive", true);
			Destroy(transform.GetChild(0).GetComponent<Collider>());
			stateChange.Invoke(true);
		}
	}
}
