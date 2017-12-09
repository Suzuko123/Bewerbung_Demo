using UnityEngine;

public class FallingRock : MonoBehaviour, IPoolable
{
    void IPoolable.Spawn()
    {
        gameObject.SetActive(true);
    }

    void IPoolable.Despawn()
    {
        gameObject.SetActive(false);
    }
}
