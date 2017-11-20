using UnityEngine;

//Author: William Rapprich
//Last edited: 20.11.2017 by: William

/// <summary>
/// Heals player by stepping into it.
/// </summary>
public class HealthPack : MonoBehaviour
{
	[SerializeField][Range(1,10)]
	int healAmount = 2;

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player"))
		{
		    //Negative damage equals heal
			other.GetComponent<IHitable> ().Hit (new HitInfo(gameObject, -healAmount));
			Destroy(gameObject);
		}
	}
}
