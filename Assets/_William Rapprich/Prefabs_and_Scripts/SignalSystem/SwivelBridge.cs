using System.Collections;
using UnityEngine;

public class SwivelBridge : Activatable
{
	enum Direction
	{
		clockwise = 1,
		counterclockwise = -1
	}

	[SerializeField] float rotationTime = 2f;
	[SerializeField] Direction rotationalDirection = Direction.clockwise;

    protected override void OnSignalChange(bool active)
    {
		if (active && !IsActive)
		{
			foreach (Activator activator in activators)
			{
				if (!activator.IsActive) return;
			}
			IsActive = true;
			StartCoroutine(Turn());
		}
		else if (!active && IsActive)
		{
			IsActive = false;
		}
    }

	IEnumerator Turn()
	{
		float time = 0f;

		while (time < rotationTime)
		{
			yield return null;
			transform.Rotate((int)rotationalDirection * Vector3.up, Time.deltaTime/rotationTime * 90);
			time += Time.deltaTime;
		}

		time -= rotationTime;
		transform.Rotate((int)rotationalDirection * -1 * Vector3.up, time/rotationTime * 90);
	}
}
