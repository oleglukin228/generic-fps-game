using System.Collections.Generic;
using UnityEngine;

public sealed class ParticleShooter : MonoBehaviour
{
    private List<ParticleCollisionEvent> _events = new();

    [SerializeField] private ParticleSystem _particleSystem;

    private void OnParticleCollision(GameObject other)
    {
        if (!other.TryGetComponent<CollisionReaction>(out var collisionReaction))
            return;
        var count = _particleSystem.GetCollisionEvents(other, _events);
        for (int i = 0; i < count; i++) 
        { 
            var collisionEvent = _events[i];
            var position = collisionEvent.intersection;
            var normal = collisionEvent.normal;
            collisionReaction.ReactToHit(position, normal);
        }
    }
}
