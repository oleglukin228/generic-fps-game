using UnityEngine;

public class DebugCursorTracker : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] WeaponManagerFullBody wpnManager;

    private void Start()
    {
        if (player == null)
            player = Object.FindAnyObjectByType<PlayerController>();
        if (wpnManager == null)
            wpnManager = Object.FindAnyObjectByType<WeaponManagerFullBody>();
    }

    private void Update()
    {
        transform.position = player.PlayerCamera.WorldToScreenPoint(wpnManager.cursor.position);
    }
}
