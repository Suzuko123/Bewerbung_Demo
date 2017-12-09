using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveableBlock : MonoBehaviour
{
	public static Dictionary<string, BlockStartMoveEvent> OnBlockStartMoveEventList = new Dictionary<string, BlockStartMoveEvent>();
	public static Dictionary<string, BlockStopMoveEvent> OnBlockStopMoveEventsList = new Dictionary<string, BlockStopMoveEvent>();

	public int Mass {get { return _mass; }}
	[SerializeField, Range(1,2)]private int _mass=1;
	[SerializeField, Tooltip("Half the size of the block")] private Vector3 _halfExtents;
	[SerializeField] private float _moveSpeed=1;
	[SerializeField] private float _movedUnitsPerPush = 1;
	[SerializeField] private float _petLength = 1.2f;
	private LayerMask _mask;
	private bool _isPulled;
	private int _movers;
	private bool _moving;
	private BlockStartMoveEvent _blockStartMoveEvent = new BlockStartMoveEvent();
	private BlockStopMoveEvent _blockStopMoveEvent = new BlockStopMoveEvent();

	public void Start()
	{
		_mask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Pet"));
		OnBlockStartMoveEventList.Add(name, _blockStartMoveEvent);
		OnBlockStopMoveEventsList.Add(name, _blockStopMoveEvent);
	}

	public void TryMoveBlock()
	{
		if (Mass <= _movers)
			MoveBlock();
	}

	public void PushStart()
	{
		_movers++;
	}

	public void PushStop()
	{
		_movers--;
	}

	private void MoveBlock()
	{
		if (!_moving)
		{
			Vector3 direction = GetMoveDirection();
			RaycastHit hit;
			if (_isPulled)
			{
				if (!Physics.BoxCast(transform.position, _halfExtents * .9f, direction, out hit, transform.rotation, _petLength+_movedUnitsPerPush,
					_mask))
				{
					StartCoroutine(MoveOneUnit(direction));
				}
			}
			else
			{
				if (!Physics.BoxCast(transform.position, _halfExtents * .9f, direction, out hit, transform.rotation, _movedUnitsPerPush, _mask))
				{
					StartCoroutine(MoveOneUnit(direction));
				}
			}
		}
	}

	private IEnumerator MoveOneUnit(Vector3 direction)
	{
		_moving = true;
		
		float travelTime = _movedUnitsPerPush / _moveSpeed;
		Vector3 endPos = transform.position + direction*_movedUnitsPerPush;

		_blockStartMoveEvent.Invoke(this.transform.position, endPos, _moveSpeed);

		for (float i = 0; i < travelTime; i += Time.deltaTime)
		{
			this.transform.Translate(direction*Time.deltaTime*_moveSpeed);
			yield return null;
		}

		this.transform.position = endPos;

		_moving = false;
		_blockStopMoveEvent.Invoke();
	}

	private Vector3 GetMoveDirection()
	{
		return _isPulled ? GetPetPullDirection() : GetPlayerPushDirection();
	}

	private bool BoxCastInDirection(Vector3 direction, LayerMask mask)
	{
		return Physics.BoxCast(transform.position, _halfExtents * .9f, direction, transform.rotation, 1f, mask);
	}

	private Vector3 GetPetPullDirection()
	{
		Vector3 direction;
		LayerMask mask = 1<<LayerMask.NameToLayer("Pet");

		direction = transform.forward;
		if (BoxCastInDirection(direction, mask))
			return direction;

		direction = Quaternion.Euler(0, 90, 0) * direction;
		if (BoxCastInDirection(direction, mask))
			return direction;

		direction = Quaternion.Euler(0, 90, 0) * direction;
		if (BoxCastInDirection(direction, mask))
			return direction;

		direction = Quaternion.Euler(0, 90, 0) * direction;
		if (BoxCastInDirection(direction, mask))
			return direction;

		return Vector3.zero;
	}

	private Vector3 GetPlayerPushDirection()
	{
		Vector3 direction;
		LayerMask mask = 1<<LayerMask.NameToLayer("Player");

		direction = transform.forward;
		if (BoxCastInDirection(direction, mask))
			return direction * -1;

		direction = Quaternion.Euler(0, 90, 0) * transform.forward;
		if (BoxCastInDirection(direction, mask))
			return direction * -1;

		direction = Quaternion.Euler(0, 180, 0) * transform.forward;
		if (BoxCastInDirection(direction, mask))
			return direction * -1;

		direction = Quaternion.Euler(0, 270, 0) * transform.forward;
		if (BoxCastInDirection(direction, mask))
			return direction * -1;

		return Vector3.zero;
	}

	public void PullStart()
	{
		if (!_isPulled)
		{
			_isPulled = true;
			_movers++;
			StartCoroutine(GetPulled());
		}
	}

	private IEnumerator GetPulled()
	{
		while (_isPulled)
		{
			TryMoveBlock();
			yield return null;
		}
	}

	public void PullStop()
	{
		if (_isPulled)
		{
			_isPulled = false;
			_movers--;
		}
	}

	private void OnDisable()
	{
		OnBlockStartMoveEventList.Remove(name);
		OnBlockStopMoveEventsList.Remove(name);
	}
}

//Parameters: StartPosition, EndPosition, Speed
public class BlockStartMoveEvent : UnityEvent<Vector3, Vector3, float> { }

public class BlockStopMoveEvent : UnityEvent { }