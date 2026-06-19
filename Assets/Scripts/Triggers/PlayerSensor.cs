using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    public delegate void PlayerEnterEvent(PlayerController player);
    public delegate void PlayerExitEvent(Vector3 lastKnownPosition);
    public event PlayerEnterEvent OnPlayerEnter;
    public event PlayerExitEvent OnPlayerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            OnPlayerEnter?.Invoke(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            OnPlayerExit?.Invoke(other.transform.position);
        }
    }
}
