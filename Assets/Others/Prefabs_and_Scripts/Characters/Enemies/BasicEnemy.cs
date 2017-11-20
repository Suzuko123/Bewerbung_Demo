using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System;

//Author: Johannes Krause
//Last edited: 20.11.2017 by: William
public class BasicEnemy : Enemy
{
    [SerializeField]
    protected int fleeThreshold;

    new private void Start()
    {
        base.Start();
        Target = Player.Instance.transform;
    }

	new private void Update()
    {
        base.Update();
    }

    protected override Vector3 GetMovingPosition()
    {
        if (hp > fleeThreshold)
            return base.GetMovingPosition();
        else
            return GetFleeingPosition();
    }

    /// <summary>
    /// Get position to which the enemy flees from the player.
    /// </summary>
    /// <returns></returns>
    protected virtual Vector3 GetFleeingPosition()
    {
        Vector3 result;

        Vector3 fleeDir = transform.position - Target.position;
        fleeDir.Normalize();
        float length = fleeDir.magnitude;
        Ray ray = new Ray(transform.position, fleeDir);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, length))
        {
            //something in the way, get away in 90° angle
            float degrees = Vector3.SignedAngle(ray.direction, hitInfo.normal, Vector3.up);
            Debug.Log(degrees);
            if (degrees >= 0)
                fleeDir = Quaternion.AngleAxis(degrees - 90, Vector3.up) * fleeDir;
            else
                fleeDir = Quaternion.AngleAxis(degrees + 90, Vector3.up) * fleeDir;
        }
        result = transform.position + fleeDir;

        return result;
    }

    protected override void AttackTarget()
    {
        if(hp > fleeThreshold)
            base.AttackTarget();
    }

    public override Transform Target
    {
        get { return base.Target; }

        set
        {
            base.Target = value;
            hitTarget = value.GetComponent<IHitable>();
        }
    }
}