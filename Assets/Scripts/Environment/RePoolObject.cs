using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RePoolObject : MonoBehaviour
{
    [SerializeField]
    private bool repoolAfterTime = true;
    [SerializeField]
    private ObjectPoolManager.PoolType poolType = ObjectPoolManager.PoolType.GameObjects;
    [SerializeField]
    private float repoolTime = 5.0f;

    private void OnEnable()
    {
        BeginDestroy();
    }

    private void BeginDestroy()
    {
        if (repoolAfterTime)
        {
            Invoke(nameof(Repool), repoolTime);
        }
    }

    public void Repool()
    {
        CancelInvoke();
        ObjectPoolManager.ReturnObjectToPool(gameObject, poolType);
    }

    private void OnParticleSystemStopped()
    {
        CancelInvoke();
        ObjectPoolManager.ReturnObjectToPool(gameObject, poolType);
    }
}
