using UnityEngine;

public class CullingSensor : MonoBehaviour
{
    [SerializeField] EnemySensor _enemySensor;

    void Start()
    {
        _enemySensor.OnEnemyEnter += EnemySensor_OnEnemyEnter;
        _enemySensor.OnEnemyExit += EnemySensor_OnEnemyExit;
    }
    
    private void EnemySensor_OnEnemyEnter(EnemyController enemy)
    {
        enemy.EnableStateMachine();
        enemy.EnableSensors(true);
    }

    private void EnemySensor_OnEnemyExit(EnemyController enemy)
    {
        enemy.DisableStateMachine();
        enemy.EnableSensors(false);
    }
}
