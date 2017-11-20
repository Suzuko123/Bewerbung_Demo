using UnityEngine;

//Author: William Rapprich
//Last edited: 20.11.2017 by William

/// <summary>
/// Holds prefabs for instantiation (will not be needed once object pool exists)
/// </summary>
public class PrefabHolder : MonoBehaviour
{

    public static PrefabHolder Instance = null; //Singleton

    public GameObject enemyHealthBarCanvas;
    public GameObject arrowProjectile;
    public GameObject heart;
    public GameObject inGameUI;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
