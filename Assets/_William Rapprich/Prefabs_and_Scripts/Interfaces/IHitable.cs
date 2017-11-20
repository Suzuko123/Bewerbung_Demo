using UnityEngine;

// Author: William Rapprich
// Last edited: 20.11.2017 by William Rapprich

/// <summary>
/// Needs to be implemented by a script on a GameObject that can be hit.
/// </summary>
public interface IHitable
{
    /// <summary>
    /// Specifies what happens if the object is hit by something.
    /// </summary>
    /// <param name="info">Collection of attributes for the hit</param>
    void Hit(HitInfo info); 
}

/// <summary>
/// Collection of attributes for a hit
/// </summary>
public class HitInfo
{
    /// <summary>
    /// Amount of damage inflicted by the hit source.
    /// </summary>
    public int damage = 0;

    /// <summary>
    /// GameObject issuing the hit.
    /// </summary>
    public GameObject source;

    public Vector3 impulse = Vector3.zero;

    public HitInfo(GameObject source, int damage)
    {
        this.source = source;
        this.damage = damage;
    }

    public HitInfo(GameObject source, int damage, Vector3 impulse)
    {
        this.source = source;
        this.damage = damage;
        this.impulse = impulse;
    }
}
