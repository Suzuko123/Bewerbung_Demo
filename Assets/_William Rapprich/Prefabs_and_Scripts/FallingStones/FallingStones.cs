using UnityEngine;
using System.Collections;

//Author: William Rapprich
//Last edited: 6.12.2017 by: William
[ExecuteInEditMode]
public class FallingStones : MonoBehaviour
{
	[SerializeField] float width = 50f, height = 50f, depth = 50f;
	[SerializeField] Color boxColor = Color.cyan;
	[SerializeField] float spawnInterval = 0.5f;
	[SerializeField] float dangerZoneActiveTime = 2f;
	[SerializeField] float dangerZoneRadius = 2f;
	[SerializeField] float radialVariety = 1f;
	[SerializeField] GameObject stonePrefab;
	[SerializeField] int damage = 2;
	LayerMask playerLayer;

	void Start()
	{
		if (Application.isPlaying)
		{
			StartCoroutine(Spawn());
			playerLayer = 1 << LayerMask.NameToLayer("Player");
		}
	}

	void Update()
	{
		if (!Application.isPlaying)
		{
			DrawBox();
		}
	}

	IEnumerator Spawn()
	{
		while (gameObject.activeSelf)
		{
			yield return new WaitForSeconds(spawnInterval);

			for(int i = 0; i < 3; i++) //look for spawn position 3 times max
			{
				RaycastHit raycastHit;
				Vector3 startPoint = transform.position + Random.Range(0f, width) * transform.right + Random.Range(0f, depth) * transform.forward;
				if (Physics.Raycast( startPoint, -transform.up, out raycastHit, height) )
				{
					//only spawn if spawn point is visible
					Vector3 screenPos = Camera.main.WorldToViewportPoint(raycastHit.point);
					if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1) 
						break;
					else
					{
						StartCoroutine(Fall(raycastHit.point));
						break;
					}
				}
			}
		}
	}

	IEnumerator Fall(Vector3 center)
	{
		//variables for setup
		float radius = dangerZoneRadius + Random.Range(-radialVariety, +radialVariety);
		float fallHeight = Mathf.Abs(Physics.gravity.y) * Mathf.Pow(dangerZoneActiveTime, 2) / 2;

		//stone setup for reuse
		Transform stoneTrans = PrefabPool.SpawnClone(stonePrefab).transform;
		stoneTrans.GetComponent<Rigidbody>().velocity = Vector3.zero;
		stoneTrans.position = center + transform.up * fallHeight;
		stoneTrans.localScale = new Vector3(radius,radius,radius);
		stoneTrans.parent = transform;

		//danger zone setup for reuse
		Transform zone = stoneTrans.GetChild(1);
		zone.position = center;
		zone.parent = transform; //remove static child from moving parent

		//material setup for reuse
		Material mat = zone.GetComponent<MeshRenderer>().material;
		mat.SetFloat("_FillPercent", 0f);

		float time = 0f;
		while (time < dangerZoneActiveTime)
		{
			yield return null;
			time += Time.deltaTime;
			mat.SetFloat("_FillPercent", time/dangerZoneActiveTime);
		}

		if (Physics.OverlapSphere(center, radius, playerLayer).Length > 0) 
			Player.Instance.Hit(new HitInfo(gameObject,damage));

		zone.parent = stoneTrans; //reparent static child for reuse
		PrefabPool.DespawnClone(stoneTrans.gameObject);
	}

	void DrawBox()
	{
		Debug.DrawLine(transform.position, transform.position + transform.right * width, boxColor);
		Debug.DrawLine(transform.position, transform.position - transform.up * height, boxColor);
		Debug.DrawLine(transform.position, transform.position + transform.forward * depth, boxColor);
		Debug.DrawLine(transform.position + transform.right * width, transform.position + transform.right * width - transform.up * height, boxColor);
		Debug.DrawLine(transform.position + transform.right * width, transform.position + transform.right * width + transform.forward * depth, boxColor);
		Debug.DrawLine(transform.position - transform.up * height, transform.position - transform.up * height + transform.right * width, boxColor);
		Debug.DrawLine(transform.position - transform.up * height, transform.position - transform.up * height + transform.forward * depth, boxColor);
		Debug.DrawLine(transform.position + transform.forward * depth, transform.position + transform.forward * depth + transform.right * width, boxColor);
		Debug.DrawLine(transform.position + transform.forward * depth, transform.position + transform.forward * depth - transform.up * height, boxColor);
		Debug.DrawLine(transform.position + transform.right * width - transform.up * height, transform.position + transform.right * width - transform.up * height + transform.forward * depth, boxColor);
		Debug.DrawLine(transform.position + transform.right * width + transform.forward * depth, transform.position + transform.right * width - transform.up * height + transform.forward * depth, boxColor);
		Debug.DrawLine(transform.position + transform.forward * depth - transform.up * height, transform.position + transform.right * width - transform.up * height + transform.forward * depth, boxColor);
	}
}