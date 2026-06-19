using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    public delegate void EnemyEnterEvent(EnemyController enemy);
    public delegate void EnemyExitEvent(EnemyController enemy);
    public event EnemyEnterEvent OnEnemyEnter;
    public event EnemyExitEvent OnEnemyExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyController enemy))
        {
            OnEnemyEnter?.Invoke(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out EnemyController enemy))
        {
            OnEnemyExit?.Invoke(enemy);
        }
    }
}
