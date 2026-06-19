using UnityEngine;
interface IDamageable
{
    void TakeDamage(float amount, Vector3 direction, Vector3 hitPoint);
    void Die(Vector3 direction, Vector3 hitPoint);
}
