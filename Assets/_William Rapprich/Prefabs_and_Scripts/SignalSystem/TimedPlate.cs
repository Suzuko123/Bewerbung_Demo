using UnityEngine;
using System.Collections;

//Author: William Rapprich
//Last edited: 29.11.2017 by William
public class TimedPlate : Activator
{
	[SerializeField] float activatedTime = 5f;

    void OnTriggerEnter(Collider other)
	{
		if (!IsActive && (other.CompareTag("Player") || other.CompareTag("Pet")))
		{
			IsActive = true;
			anim.SetBool("isActive", true);
			stateChange.Invoke(true);
			StartCoroutine(SpringBack(activatedTime));
		}
	}

	IEnumerator SpringBack(float time)
	{
		yield return new WaitForSeconds(time);
		IsActive = false;
		anim.SetBool("isActive", false);
		stateChange.Invoke(false);
	}
}
