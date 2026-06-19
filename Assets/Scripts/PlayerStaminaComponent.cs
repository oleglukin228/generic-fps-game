using AudioSystem;
using Kryz.CharacterStats;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerStaminaComponent : HealthComponent
{
    [SerializeField] private CharacterStat regenDelay;
    [SerializeField] private CharacterStat regenRate;
    [SerializeField] private CharacterStat decreaseRate;
    [SerializeField] private PlayerController player;
    [SerializeField] private TMP_Text staminaText;
    private bool _isDyspnea;
    public float LastDamageTime {  get; private set; }
    public bool IsTired { get { if (currentHealth < 5f || _isDyspnea) return true; return false; } }
    public bool IsRegenerating { get { return LastDamageTime + regenDelay.Value <= Time.time; } }
    public bool IsFullHealth { get { return currentHealth >= maxHealth.Value; } }
    public PlayerController Player { get { return player; } }
    private void LateUpdate()
    {
        if (IsDead && IsFullHealth) return;
        if (!IsRegenerating || player.IsRunning) return;
        currentHealth += regenRate.Value * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth.Value);
    }

    private void FixedUpdate()
    {
        PostProcessingController.ChangeStaminaVolumeValue(Mathf.Lerp(1.1f, 0f, currentHealth / maxHealth.Value));
        staminaText.text = "Усталость: \n" + (maxHealth.Value - currentHealth).ToString("0");
    }

    public override void TakeDamage(float damageAmount, Vector3 shootDir, Vector3 hitPoint)
    {
        base.TakeDamage(damageAmount, shootDir, hitPoint);
        LastDamageTime = Time.time;
    }

    public override void TakeDamage(float damageAmount, Vector3 shootDir, Vector3 hitPoint, string bodyPart)
    {
        base.TakeDamage(damageAmount, shootDir, hitPoint, bodyPart);
        LastDamageTime = Time.time;
    }

    public override void TakeDamage(float damageAmount, Vector3 shootDir, Vector3 hitPoint, string bodyPart, Rigidbody rb)
    {
        base.TakeDamage(damageAmount, shootDir, hitPoint, bodyPart, rb);
        LastDamageTime = Time.time;
    }

    public void StopRegenerating() => LastDamageTime = Time.time;

    public void DecreaseHealth()
    {
        currentHealth -= decreaseRate.Value * Time.deltaTime;
        if (IsTired)
        {
            LastDamageTime = Time.time;
            _isDyspnea = true;
            player.PlayerSFX.ChangeStaminaSound(player.PlayerSFX.tiredBreathSound);
            StartCoroutine(StartDyspnea());
        } 
    }

    private IEnumerator StartDyspnea()
    {
        while (currentHealth < 50f) yield return null;
        player.PlayerSFX.ChangeStaminaSound(player.PlayerSFX.calmBreathSound);
        _isDyspnea = false;
    }
}
