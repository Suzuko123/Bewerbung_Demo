using System.Collections;
using UnityEngine;

//Author: William Rapprich
// Last modified: 20.11.2017 by William

/// <summary>
/// Keeps track of every enemy in the scene and manages enemy health bar culling so far.
/// </summary>
public class EnemyPool : MonoBehaviour
{
    int numberOfEnemies = 0;
    CullingGroup cullingGroup = null;
    EnemyInfo[] enemies;
    BoundingSphere[] boundingSpheres; //used in cullingGroup for tracking visibility changes

    /// <summary>
    /// Represents an active enemy supposed to be kept track of.
    /// </summary>
    class EnemyInfo
    {
        public Enemy script;
        public int sphereIndex;

        public EnemyInfo(Enemy e, int i)
        {
            script = e;
            sphereIndex = i;
        }
    }

	/// <summary>
	/// Global singleton instance of EnemyCulling
	/// </summary>
	public static EnemyPool Instance { get; private set; }

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

		enemies = new EnemyInfo[50];

		cullingGroup = new CullingGroup();
		cullingGroup.targetCamera = Camera.main;

		boundingSpheres = new BoundingSphere[50];
		cullingGroup.SetBoundingSpheres(boundingSpheres);
		cullingGroup.SetBoundingSphereCount(0);

		cullingGroup.onStateChanged = OnVisibilityChange;

		StartCoroutine(UpdateSpheres());
	}

	/// <summary>
    /// Registers enemy to pool.
    /// </summary>
    /// <param name="newEnemy">Caller of the method; any script inheriting from Enemy</param>
    public void Register(Enemy newEnemy)
    {
        //Find out whether enemy is already registered,
        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (enemies[i].script == newEnemy)
                return;
        }
        // Add enemy to enemies list and add a new boundingsphere to be assigned to it.
        enemies[numberOfEnemies] = new EnemyInfo(newEnemy, numberOfEnemies);
        boundingSpheres[numberOfEnemies] = new BoundingSphere(newEnemy.gameObject.transform.position, newEnemy.GetBoundingSphereRadius());

        numberOfEnemies++;
        cullingGroup.SetBoundingSphereCount(numberOfEnemies);
    }

	/// <summary>
    /// Removes enemy from pool.
    /// </summary>
    /// <param name="enemy">Caller of the method; any script inheriting from Enemy</param>
    public void Remove(Enemy enemy)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            if(enemies[i].script == enemy)
            {
                numberOfEnemies--;
                
                //bring last sphere in array to the index of the enemy to be removed
                cullingGroup.EraseSwapBack(i); 

                //swap enemy to be removed for last enemy in array
                enemies[i] = enemies[numberOfEnemies];
                enemies[i].sphereIndex = i;
                enemies[numberOfEnemies] = null;

                cullingGroup.SetBoundingSphereCount(numberOfEnemies);
                break;
            }
        }
    }

	/// <summary>
    /// Called by culling group on visibility state change for each sphere.
    /// </summary>
    void OnVisibilityChange(CullingGroupEvent sphere)
    {
        if(sphere.hasBecomeVisible)
        {
            enemies[sphere.index].script.Show();
        }
        else if(sphere.hasBecomeInvisible)
        {
            enemies[sphere.index].script.Cull();
        }
    }

	/// <summary>
    /// Called each frame to synchronize enemies and boundingSpheres arrays.
    /// </summary>
    IEnumerator UpdateSpheres()
    {
        while(Instance != null)
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                boundingSpheres[i].position = enemies[i].script.gameObject.transform.position;
            }
            yield return null;
            yield return null; //Updating every 2 frames suffices
        }
    }

    /// <summary>
    /// Kills all registered enemies
    /// </summary>
	public void KillAll()
    {
        //new array needed because order of the original is not preserved after removing an enemy
        Enemy[] enemiesToKill = new Enemy[numberOfEnemies];

        for(int i = 0; i< numberOfEnemies; i++)
        {
            enemiesToKill[i] = enemies[i].script;
        }

        foreach(Enemy enemy in enemiesToKill)
        {
            enemy.Hit(new HitInfo(null,100));
        }
    }

    //Reset singleton instance on reload
    void OnDestroy()
	{
		Instance = null;
		
        // free memory
        if(cullingGroup != null)
        {
            cullingGroup.Dispose();
            cullingGroup = null;
        }
	}
}
