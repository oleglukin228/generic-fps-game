using UnityEngine;
using UnityEngine.UI;

public class HitboxSensor : MonoBehaviour
{
    public delegate void HitboxEnterEvent(Hitbox rb);
    public delegate void HitboxExitEvent(Hitbox rb);
    public event HitboxEnterEvent OnHitboxEnter;
    public event HitboxExitEvent OnHitboxExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Hitbox hitbox))
        {
            Debug.Log(other.name);
            OnHitboxEnter?.Invoke(hitbox);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Hitbox hitbox))
        {
            OnHitboxExit?.Invoke(hitbox);
        }
    }
}
