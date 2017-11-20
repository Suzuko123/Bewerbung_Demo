using System;
using System.Collections;
using UnityEngine;

//Author: William Rapprich
//Last edited: 20.11.2017 by: William

/// <summary>
/// Arrow Trap behaviour with three different modes.
/// </summary>
public class ArrowTrap : MonoBehaviour
{
	[Tooltip("Die Größe gibt an, wie viele Intervalle es für das Muster gibt und die Elemente spezifizieren die jeweiligen Intervalle, nach denen alle Pfeile geschossen werden.")]
	[SerializeField] float[] shotPattern;
	
	[Tooltip("Bei Shot Pattern Size 5 bekommt jedes Schussloch ein Intervall aus dem Array zugewiesen, mit dem es Pfeile schießt.")]
	[SerializeField] bool patternSwitch = false;

	[Tooltip("Wenn Pattern Switch gesetzt ist, werden Pfeile in einem Gesamtintervall entsprechend der größten Zahl im Array nach den jeweiligen Sekunden geschossen")]
	[SerializeField] bool normalizedStart = false;
	[SerializeField] float velocity = 25f;
	[SerializeField] float range = 10f;
	[SerializeField] int damage = 1;
	
	Transform[] spawnPoints;
	GameObject arrow;

	void Start ()
	{
		arrow = PrefabHolder.Instance.arrowProjectile;

		//Get all 5 spawnpoints
		spawnPoints = new Transform[5];
		int i = 0;
		foreach (Transform point in transform.GetChild(2))
		{
			spawnPoints[i] = point;
			i++;
		}

		//turn of pattern switch if not exactly 5 intervals provided
		if (shotPattern.Length != 5)
			patternSwitch = false;

		if (!patternSwitch)
		{
			//add up intervals
			float wholeInterval = 0;
			foreach (float t in shotPattern)
			{
				wholeInterval += t;
			}

			float timePassed = 0f;
			foreach (float interval in shotPattern)
			{
				InvokeRepeating("ShootAll", timePassed, wholeInterval);
				timePassed += interval;
			}
		}
		else
		{
			if (normalizedStart)
			{
				float wholeInterval = 0;
				foreach (float t in shotPattern)
				{
					if (wholeInterval < t)
						wholeInterval = t;
				}
				StartCoroutine(ShootNormalized(wholeInterval));
			}
			else
			{
				StartCoroutine(ShootSingleIntervals());
			}
		}
	}
	
	/// <summary>
	/// Shoot all arrows at the same time.
	/// </summary>
	void ShootAll()
	{
		foreach (Transform origin in spawnPoints)
		{
			Instantiate(arrow, origin.position, origin.rotation, transform)
			.GetComponent<EnemyProjectile>().SetAttributes(velocity, range, damage);
			
		}
	}

	/// <summary>
	/// Shoots arrows with each spawnpoint shooting at a specific time in a total interval.
	/// </summary>
	/// <param name="wholeInterval">Total interval in which all spawnpoints fire their shot once.</param>
	IEnumerator ShootNormalized(float wholeInterval)
	{
		while(gameObject.activeSelf)
		{
			for(int i=0; i<5; i++)
			{
				shotPattern[i] -= Time.deltaTime;
				if (shotPattern[i] <= 0f)
				{
					Instantiate(arrow, spawnPoints[i].position, spawnPoints[i].rotation, transform)
					.GetComponent<EnemyProjectile>().SetAttributes(velocity, range, damage);
					shotPattern[i] += wholeInterval;
				}
			}
			yield return null;
		}
	}

	/// <summary>
	/// Shoots arrows with each spawnpoint having their own shot intervals.
	/// </summary>
	IEnumerator ShootSingleIntervals()
	{
		float[] array = new float[5];
		Array.Copy(shotPattern, array, 5);

		while(gameObject.activeSelf)
		{
			for(int i=0; i<5; i++)
			{
				array[i] -= Time.deltaTime;
				if (array[i] <= 0f)
				{
					Instantiate(arrow, spawnPoints[i].position, spawnPoints[i].rotation, transform)
					.GetComponent<EnemyProjectile>().SetAttributes(velocity, range, damage);
					array[i] += shotPattern[i];
				}
			}
			yield return null;
		}
	}
}
