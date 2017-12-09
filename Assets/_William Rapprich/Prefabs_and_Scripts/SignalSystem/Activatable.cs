using UnityEngine;

//Author: William Rapprich
//Last edited: 29.11.2017 by William
public abstract class Activatable : MonoBehaviour
{
	[Tooltip("Activators used for activation/deactivation")]
	[SerializeField] protected Activator[] activators;
	public bool IsActive{get; protected set;}
	protected Animator anim;

	virtual protected void Awake()
	{
		anim = GetComponent<Animator>();

		foreach(Activator activator in activators)
		{
			activator.stateChange.AddListener(OnSignalChange);
		}
	}

	abstract protected void OnSignalChange(bool active);
}
