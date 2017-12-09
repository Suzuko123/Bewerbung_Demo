using System;
using UnityEngine;
using System.Collections.Generic;

public class Pool
{
	private GameObject _prefab;
	private Transform _poolParent;
	private Queue<GameObject> _inactive;
	private List<GameObject> _active;

	public Pool(GameObject prefab)
	{
		_prefab = prefab;
		_inactive = new Queue<GameObject>();
		_active = new List<GameObject>();
		_poolParent = new GameObject(string.Format("{0}_Pool",_prefab.name)).transform;
		GameObject.DontDestroyOnLoad(_poolParent);
	}

	public GameObject GetClone()
	{
		GameObject go;

		if (_inactive.Count > 0)
			go = _inactive.Dequeue();
		else
		{
			go = GameObject.Instantiate(_prefab);
			go.transform.SetParent(_poolParent);
		}

		_active.Add(go);

		return go;
	}

	public void ReturnClone(GameObject go)
	{
		if (_active.Remove(go))
			_inactive.Enqueue(go);
		else
			Debug.LogError(string.Format("GameObject {0} is not managed by the pool.", go));
	}
}
