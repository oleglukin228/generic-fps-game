using UnityEngine;

public class RoomData : MonoBehaviour
{
    public Transform wallsParent, entitiesParent, roomDecorationsParent, floorDecorationsParent, backroundDecorationsParent;
    public Vector3Int roomCenter;
    public EnemyController[] enemies;
    public BoxCollider occlusionTrigger;
    public PlayerSensor playerSensor;
    bool _isSquadAlerted;

    private void Start()
    {
        FindEnemiesInRoom();
        //SetupTrigger();
        //playerSensor.OnPlayerEnter += PlayerSensor_OnPlayerEnter;
        //playerSensor.OnPlayerExit += PlayerSensor_OnPlayerExit;
    }

    private void PlayerSensor_OnPlayerEnter(PlayerController player)
    {
        /*CancelInvoke(nameof(DisableSMAfterTimer));
        foreach (var enemy in enemies)
        {
            enemy.EnableStateMachine();
        }*/
    }
    
    private void PlayerSensor_OnPlayerExit(Vector3 lastKnownPosition)
    {
        //Invoke(nameof(DisableSMAfterTimer), 10f);
    }

    void DisableSMAfterTimer()
    {
        foreach (var enemy in enemies)
        {
            enemy.DisableStateMachine();
        }
    }

    public void FindEnemiesInRoom()
    {
        enemies = entitiesParent.gameObject.GetComponentsInChildren<EnemyController>(true);
        foreach (EnemyController controller in enemies)
        {
            if (controller.CompareTag("IgnoreSquad")) continue;
            controller.Health.OnDamageEvent += AlertSquad;
            controller.Health.OnDeathEvent += RemoveSquadCrew;
            //controller.DisableStateMachine();
            //GameEnvironment.AddEntity(controller.gameObject);
        }
    }

    private void RemoveSquadCrew()
    {
        
    }

    public void AlertSquad()
    {
        if (_isSquadAlerted) return;
        _isSquadAlerted = true;

        foreach (var enemy in enemies)
        {
            if (!enemy.StateMachine.IsInCurrentState(EEnemyState.Death)) 
            {
                enemy.StateMachine.SetState(EEnemyState.Chasing);
                enemy.Health.OnDamageEvent -= AlertSquad;
            }
        }
    }

    private void SetupTrigger()
    {
        occlusionTrigger.center = roomCenter;
        occlusionTrigger.size = new Vector3Int(10, 4, 10);
    }
}
