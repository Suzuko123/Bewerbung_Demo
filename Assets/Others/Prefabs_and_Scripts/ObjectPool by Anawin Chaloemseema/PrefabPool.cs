using System;
using System.Collections.Generic;
using UnityEngine;

//Assuming only one component on gameObject implements IPoolable
public class PrefabPool : MonoBehaviour
{
	private static Dictionary<GameObject, Pool> _pools = new Dictionary<GameObject, Pool>();
	private static Dictionary<GameObject, Pool> _activeObjects = new Dictionary<GameObject, Pool>();

#if UNITY_EDITOR
	public static int ActiveObjects { get { return _activeObjects.Count; } }
#endif

	public static GameObject SpawnClone(GameObject prefab)
	{
		if (!_pools.ContainsKey(prefab))
			_pools.Add(prefab, new Pool(prefab));

		GameObject clone = _pools[prefab].GetClone();
		_activeObjects.Add(clone, _pools[prefab]);
		clone.GetComponent<IPoolable>().Spawn();

		return clone;
	}

	public static void DespawnClone(GameObject clone)
	{
        if (_activeObjects.ContainsKey(clone))
        {
            clone.GetComponent<IPoolable>().Despawn();
            _activeObjects[clone].ReturnClone(clone);
            _activeObjects.Remove(clone);
        }
        else
        {
            Destroy(clone);
        }
	}
}
