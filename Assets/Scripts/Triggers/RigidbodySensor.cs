using UnityEngine;

public class RigidbodySensor : MonoBehaviour
{
    public delegate void RigidbodyEnterEvent(Rigidbody rb);
    public delegate void RigidbodyExitEvent(Rigidbody rb);
    public event RigidbodyEnterEvent OnHitboxEnter;
    public event RigidbodyExitEvent OnHitboxExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody)
        {
            OnHitboxEnter?.Invoke(other.attachedRigidbody);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody)
        {
            OnHitboxExit?.Invoke(other.attachedRigidbody);
        }
    }
}
