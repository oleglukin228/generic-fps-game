using System.Collections.Generic;
using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    [SerializeField] GunItem gun;
    protected List<ParticleCollisionEvent> events = new();
    private void OnParticleCollision(GameObject other)
    {
        other.TryGetComponent<Collider>(out var collider);
        var count = gun.particleShooter.GetCollisionEvents(other, events);
        for (int i = 0; i < count; i++)
        {
            var collisionEvent = events[i];
            var position = collisionEvent.intersection;
            var hitNormal = collisionEvent.normal;
            SurfaceManager.Instance.SpawnEffect(position, Quaternion.FromToRotation(Vector3.forward, hitNormal), -hitNormal, collider, gun.surfaceImpact, collider.transform);
            if (other.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(gun.CurrentStats.damage, position, hitNormal);
        }
    }
}
