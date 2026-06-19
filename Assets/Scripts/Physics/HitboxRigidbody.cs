using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitboxRigidbody : Hitbox, IDamageable
{
    [SerializeField] protected Rigidbody _rb;

    protected override void Start()
    {
        _rb = GetComponent<Rigidbody>();
        health = GetComponentInParent<HealthComponent>();
    }

    public override void TakeDamage(float amount, Vector3 direction, Vector3 hitPoint)
    {
        if (!health.IsDead) health.TakeDamage(amount * bodyPartInfo.damageMultiplier, direction, hitPoint, bodyPartInfo.bodyPartName, _rb);
        else _rb.AddForceAtPosition(transform.position - direction, hitPoint, ForceMode.Impulse);
    }

    public override void Die(Vector3 direction, Vector3 hitPoint)
    {
        
    }
}
