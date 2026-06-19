using AudioSystem;
using Kryz.CharacterStats;
using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] protected CharacterStat maxHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] SoundData _hitSound;
    [SerializeField] SoundData _deathSound;
    private Animator _animator;
    public delegate void onDamageEvent();
    public delegate void onDeathEvent();
    public event onDamageEvent OnDamageEvent;
    public event onDeathEvent OnDeathEvent;

    public bool IsDead { get { if (currentHealth < 0) return true; return false; } }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        currentHealth = maxHealth.BaseValue;
    }

    public virtual void TakeDamage(float damageAmount, Vector3 shootDir, Vector3 hitPoint)
    {
        OnDamageEvent?.Invoke();
        currentHealth -= damageAmount;
        if (IsDead) Die(shootDir, hitPoint, currentHealth <= -maxHealth.Value * 10f);
    }

    public virtual void TakeDamage(float damageAmount, Vector3 shootDir, Vector3 hitPoint, string bodyPart)
    {
        OnDamageEvent?.Invoke();
        currentHealth -= damageAmount;
        _animator.SetTrigger("GetHit" + bodyPart);
        if (IsDead) Die(shootDir, hitPoint, currentHealth <= -maxHealth.Value * 10f);
    }

    public virtual void TakeDamage(float damageAmount, Vector3 shootDir, Vector3 hitPoint, string bodyPart, Rigidbody rb)
    {
        OnDamageEvent?.Invoke();
        currentHealth -= damageAmount;
        _animator.SetTrigger("GetHit" + bodyPart);
        if (IsDead) Die(shootDir, hitPoint, currentHealth <= -maxHealth.Value * 10f, rb);
    }

    private void Die(Vector3 shootDir, Vector3 hitPoint, bool b)
    {
        OnDeathEvent?.Invoke();
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(_deathSound);
    }

    private void Die(Vector3 shootDir, Vector3 hitPoint, bool b, Rigidbody rb)
    {
        OnDeathEvent?.Invoke();
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(_deathSound);
        rb.AddForceAtPosition(transform.position - shootDir, hitPoint, ForceMode.Impulse);
    }
}
