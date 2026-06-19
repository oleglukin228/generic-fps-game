using System;
using System.Collections;
using System.Runtime.InteropServices;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

public class Maksim228Controller : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private HealthComponent _health;
    [SerializeField] private PlayerSensor _attackSensor;
    [SerializeField] SurfaceImpact _onAttackSurfaceImpact;
    private NavMeshAgent _agent;
    [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    static extern int MessageBoxA(IntPtr hwnd, [MarshalAs(UnmanagedType.LPStr)] string text, [MarshalAs(UnmanagedType.LPStr)] string caption, uint type);

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _health = GetComponent<HealthComponent>();
        _player = FindAnyObjectByType<PlayerController>();

        _agent.updatePosition = true;
        _agent.updateRotation = false;
    }

    private void Start()
    {
        _attackSensor.OnPlayerEnter += AttackSensor_OnPlayerEnter;
        _attackSensor.OnPlayerExit += AttackSensor_OnPlayerExit;
    }

    private void Update()
    {
        _agent.SetDestination(_player.transform.position);
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = _player.PlayerCamera.transform.position;
        cameraPosition.y = transform.position.y;
        transform.LookAt(cameraPosition);
    }

    private void AttackSensor_OnPlayerEnter(PlayerController player)
    {
        if (player.TryGetComponent<IDamageable>(out var damageable))
        {
            MessageBoxA(IntPtr.Zero, "COCU XYU PUDOPAC", "scp maksim228", 0);
            SurfaceManager.Instance.SpawnEffect(player.transform.position, Quaternion.identity, Vector3.zero, player.Hitbox, _onAttackSurfaceImpact, null);
            //damageable.TakeDamage(999999, Vector3.zero, Vector3.zero);
            StartCoroutine(KTo_TTpo4uTaJl_ToT_JloX());
        }
    }

    private IEnumerator KTo_TTpo4uTaJl_ToT_JloX()
    {
        yield return new WaitForSeconds(0.1f);
        UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
    }

    private void AttackSensor_OnPlayerExit(Vector3 position)
    {
        
    }
}
