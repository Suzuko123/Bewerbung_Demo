using UnityEngine;
using System.Collections.Generic;

//Author: William Rapprich
//Last edited: 21.11.2017 by William

public class Room : MonoBehaviour
{
    List<Enemy> enemies; 
	[SerializeField] Animator[] animators;

	void Start ()
	{
		enemies = new List<Enemy>();
        Invoke("Setup", 2);
	}
	
	void OnEnemyHit(Enemy enemy, int currentHp, int startHp)
	{
        if (animators != null)
        {
            if (currentHp <= 0)
            {
                enemies.Remove(enemy);
                if (enemies.Count == 0)
                {
                    foreach (Animator anim in animators)
                    {
                        anim.SetBool("isActive", true);
                    }
                    Destroy(this);
                }
            }
        }
	}

    void Setup()
    {
        Transform enemyTransform = transform.Find("Enemies");
        if (enemyTransform != null)
        {
            foreach (Transform child in enemyTransform)
            {
                enemies.Add(child.GetComponent<Enemy>());
                enemies[enemies.Count - 1].healthChange.AddListener(OnEnemyHit);
            }
        }
    }

	void Clear()
	{
		Enemy[] tempEnemies = new Enemy[enemies.Count];

		for (int i = 0; i< enemies.Count; i++)
		{
			tempEnemies[i] = enemies[i];
		}

		for (int i = 0; i< tempEnemies.Length; i++)
		{
			tempEnemies[i].Hit(new HitInfo(Player.Instance.gameObject, 100));
		}

		enemies.Clear();
		Destroy(this);
	}
}
