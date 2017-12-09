using UnityEngine;
using UnityEngine.Events;

//Author: William Rapprich
//Last edited: 29.11.2017 by William
public abstract class Activator : MonoBehaviour
{
	[HideInInspector] public StateChangeEvent stateChange = new StateChangeEvent();
	public bool IsActive{get; protected set;}
	protected Animator anim;

	virtual protected void Awake()
	{
		anim = GetComponent<Animator>();
	}
}

public class StateChangeEvent: UnityEvent<bool>
{}
