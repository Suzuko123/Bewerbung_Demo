using UnityEngine;

 //Author: William Rapprich
 //last edited: 20.11.2017 by William Rapprich

 /// <summary>
 /// Kills player if they enter collider.
 /// </summary>
public class DeathTrap : MonoBehaviour
{
	void OnTriggerEnter(Collider coll)
	{
		if (coll.CompareTag("Player"))
		{
			Player.Instance.Hit(new HitInfo(gameObject, 100));
		}
	}
}
