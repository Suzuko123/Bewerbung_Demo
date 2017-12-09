using UnityEngine;

//Author: William Rapprich
//Last edited: 29.11.2017 by William
public class RepeatedlyActivatable : Activatable
{
	override protected void OnSignalChange(bool active)
	{
		if (active && !IsActive)
		{
			foreach (Activator activator in activators)
			{
				if (!activator.IsActive) return;
			}
			IsActive = true;
			anim.SetBool("isActive", true);
		}
		else if (!active && IsActive)
		{
			IsActive = false;
			anim.SetBool("isActive", false);
		}
	}
}
