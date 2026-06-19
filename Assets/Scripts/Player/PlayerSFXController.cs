using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StaminaSoundState
{
    Idle,
    RunningBreath,
    TiredBreath,
    RecoveringBreath,
    Cough
}

public class PlayerSFXController : MonoBehaviour
{
    [Header("Sound and effects")]
    public AudioClip calmBreathSound;
    public AudioClip tiredBreathSound;
    public AudioClip knockdownBreathSound;
    public AudioClip recoveringBreathSound;
    public AudioClip coughSound;
    public SoundData knockdownSound;
    public SoundData rapedSound;
    public SoundData pickupSound;
    public SoundData putdownSound;

    public AudioSource staminaSource;
    public AudioSource heartBeatSound;

    public void ChangeStaminaSound(AudioClip staminaSound)
    {
        staminaSource.clip = staminaSound;
        staminaSource.Play();
    }

    /*float elapsedTime = 0.0f;
    public IEnumerator ChangeSound()
    {
        previousSound = currentSound;
        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(1f, 0f, elapsedTime / 0.5f);
            yield return null;
        }
        elapsedTime = 0f;
        audioSource.clip = currentSound;
        audioSource.Play();
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, elapsedTime / 0.5f);
            yield return null;
        }
    }*/
}
