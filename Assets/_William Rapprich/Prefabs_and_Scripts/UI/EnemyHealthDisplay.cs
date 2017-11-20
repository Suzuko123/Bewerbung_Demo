using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author: William Rapprich
//Last edited: 20.11.2017 by: William

/// <summary>
/// Responsible for managing all enemy health bars and displaying them at the right positions in each frame.
/// Also displays changes to enemies' HP.
/// </summary>
public class EnemyHealthDisplay : MonoBehaviour
{
	/// <summary>
	/// Bundles all information needed for managing a health bar.
	/// </summary>
	private struct HealthBarInfo
	{
		public Enemy enemy;
		public RectTransform parentRect;
		public Image healthBar;

		public HealthBarInfo(Enemy e, RectTransform r, Image i, int h)
		{
			enemy = e;
			parentRect = r;
			healthBar = i;
		}
	}
	
	/// <summary>
	/// Global singleton instance of EnemyHealthDisplay
	/// </summary>
	public static EnemyHealthDisplay Instance { get; private set; }
	
	/// <summary>
	/// A list containing information about all enemies whose health bars are displayed.
	/// </summary>
	List<HealthBarInfo> healthBars;

	/// <summary>
	/// Main camera
	/// </summary>
	Camera cam;

    [SerializeField]
    GameObject enemyHealthBarPrefab;

	void Awake()
	{
		//Singleton
		if (Instance != null && Instance != this)
		{
			// Destroy the heretic if there is another instance
			Destroy(gameObject);
			return;
		}
		Instance = this;

		cam = Camera.main;

		healthBars = new List<HealthBarInfo>();

		StartCoroutine(UpdateHealthBars());
	}
	
	/// <summary>
	/// Updates the positions of each health bar to be in sync with their gameobjects on screen.
	/// </summary>
	IEnumerator UpdateHealthBars ()
	{
		while(Instance != null)
		{
			foreach(HealthBarInfo info in healthBars)
			{
				ChangeHealthBarPos(info.parentRect, info.enemy.GetHealthBarPos());
			}
			yield return null;
			// yield return null //can be added to boost performance but at the cost of smooth health bars
		}
	}

	/// <summary>
	/// Displays new HP of an enemy upon health change event.
	/// </summary>
	/// <param name="enemy">Caller of the method; any script inheriting from Enemy</param>
	/// <param name="currentHp">Enemy's current HP</param>
	/// <param name="startHp">Enemy's original HP</param>
	void Display(Enemy enemy, int currentHp, int startHp)
	{
		//Find caller in list
		foreach(HealthBarInfo info in healthBars)
		{
			if(info.enemy == enemy)
			{
                if (currentHp >= 0)
				{
                    info.healthBar.fillAmount = (float)currentHp/startHp;
				}
				else
					info.healthBar.fillAmount = 0f;
				
				break;
			}
		}
	}

	/// <summary>
	/// Assigns an enemy to a new health bar so it can display its HP.
	/// </summary>
	/// <param name="newEnemy">Caller of the method; any script inheriting from Enemy</param>
	/// <param name="currentHp">Enemy's current HP</param>
	/// <param name="startHp">Enemy's original HP</param>
	public void Register(Enemy newEnemy, int currentHp, int startHp)
	{
		//Stop if enemy is already registered
		foreach (HealthBarInfo bar in healthBars)
		{
			if (bar.enemy == newEnemy)
				return;
		}
		//Add health bar to canvas and create list entry
		GameObject healthbar = Instantiate(enemyHealthBarPrefab, transform);
		healthBars.Add(new HealthBarInfo(newEnemy, healthbar.GetComponent<RectTransform>(), healthbar.transform.GetChild(1).GetComponent<Image>(), startHp));
		
		newEnemy.healthChange.AddListener(Display);

		HealthBarInfo info = healthBars[healthBars.Count-1]; //get new entry
		
		//initialize health bar's position and size
		ChangeHealthBarPos(info.parentRect, newEnemy.GetHealthBarPos());

		//fill bar
		info.healthBar.type = Image.Type.Filled;
		info.healthBar.fillMethod = Image.FillMethod.Horizontal;
		info.healthBar.fillAmount = (float)currentHp/startHp;
	}

	/// <summary>
	/// Removes enemy's health bar from display.
	/// </summary>
	/// <param name="enemy">Caller of the method; any script inheriting from Enemy</param>
	public void Remove(Enemy enemy)
	{
		//Find caller in list
		foreach(HealthBarInfo info in healthBars)
		{
			if(info.enemy == enemy && info.parentRect != null)
			{
				//destroy health bar and remove list entry
				Destroy(info.parentRect.gameObject);
				healthBars.Remove(info);
				break;
			}
		}
	}

	/// <summary>
	/// Changes the position of a health bar to a new one.
	/// </summary>
	/// <param name="bar">Health bar's parent RectTransform</param>
	/// <param name="position">Wanted position for the health bar.</param>
	void ChangeHealthBarPos(RectTransform bar, Vector3 position)
	{
		Vector3 barViewportPos = cam.WorldToViewportPoint(position);
		//float sizeCoeff = (30f - barViewportPos.z) * 0.01f; //becoming slightly bigger the closer the enemy is to the camera
		bar.anchorMin = barViewportPos;
		bar.anchorMax = barViewportPos;
		//bar.localScale = new Vector3(sizeCoeff, sizeCoeff, 1); Disabled for demo
	}

	void OnDestroy()
	{
		Instance = null;
	}
}
