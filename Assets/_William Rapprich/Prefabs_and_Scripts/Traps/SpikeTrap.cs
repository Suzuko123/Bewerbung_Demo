using System.Collections;
using UnityEngine;

//Author: William
//Last edited: 20.11.2017 by: William

/// <summary>
/// Spiketrap behaviour. Does damage to all IHitables that enter the collider when active.
/// </summary>
public class SpikeTrap : MonoBehaviour
{

	[SerializeField][Range(1,10)] int damage;
	[SerializeField][Range(0,5)] float speedMultiplier;
	[SerializeField][Range(0,5)] float idleTimeActive;
	[SerializeField][Range(0,5)] float idleTimeInactive;

	Animator anim;
	bool finishedAnimation = false;

	void Start ()
	{
		anim = gameObject.GetComponent<Animator>();
		anim.speed = speedMultiplier;
		StartCoroutine(Loop());
	}

	/// <summary>
	/// Loops the trap'see animation.
	/// </summary>
	IEnumerator Loop()
	{
		while(gameObject.activeSelf)
		{
			yield return new WaitForSeconds(idleTimeInactive);
			finishedAnimation = false;
			anim.SetTrigger("trigger");
			yield return new WaitUntil(()=>finishedAnimation);
			yield return new WaitForSeconds(idleTimeActive);
			finishedAnimation = false;
			anim.SetTrigger("trigger");
			yield return new WaitUntil(()=>finishedAnimation);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		IHitable hitable = other.GetComponent<IHitable>();
        if (hitable != null)
		{
            hitable.Hit(new HitInfo(gameObject,damage));
        }
	}

	public void SetReady()
	{
		finishedAnimation = true;
	}
}
