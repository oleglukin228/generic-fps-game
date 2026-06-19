using UnityEngine;

public class Hitbox : MonoBehaviour, IDamageable
{
    [SerializeField] protected HealthComponent health;
    [SerializeField] protected BodyPartSO bodyPartInfo;
    //[SerializeField] protected string bodyPart = "Body";
    //[SerializeField] protected float damageMultiplier = 1f;

    protected virtual void Start()
    {
        health = GetComponentInParent<HealthComponent>();
    }

    public virtual void TakeDamage(float amount, Vector3 direction, Vector3 hitPoint)
    {
        if (!health.IsDead) health.TakeDamage(amount * bodyPartInfo.damageMultiplier, direction, hitPoint);
    }

    public virtual void Die(Vector3 direction, Vector3 hitPoint)
    {

    }
}