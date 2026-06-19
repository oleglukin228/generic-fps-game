using AudioSystem;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public enum VectorDirection
{
    Up, 
    Down, 
    Left, 
    Right, 
    Forward, 
    Back
}

[System.Serializable]
public class ItemTweening
{
    public Transform currentItem;
    public Transform transformStartPosition;
    public Transform transformEndPosition;

    public void ResetTween()
    {
        currentItem.SetPositionAndRotation(transformStartPosition.position,
            transformStartPosition.rotation);
    }
}

[System.Serializable]
public class InteractableAction
{
    public float interactionSpeed = 2f;
    public List<ItemTweening> tweens;
    public VectorDirection vectorDirection;
    public UnityEvent onStartActionTrigger;
    public UnityEvent onEndActionTrigger;
}

public class InteractableActions : MonoBehaviour
{
    float desiredPosition;
    public List<InteractableAction> items = new List<InteractableAction>();
    int currentAction;
    float progress;
    bool canChangeAction;
    [SerializeField] bool completedAllActions;
    Coroutine playCoroutine;

    float GetVectorAxis(Vector3 direction)
    {
        switch (items[currentAction].vectorDirection)
        {
            case VectorDirection.Up: return direction.y;
            case VectorDirection.Right: return direction.x;
            case VectorDirection.Forward: return -direction.z;
            case VectorDirection.Down: return -direction.y;
            case VectorDirection.Left: return -direction.x;
            case VectorDirection.Back: return direction.z;
        }
        return -1;
    }

    void TryToChangeAction()
    {
        if (progress < 0.999f && progress > 0.001f) canChangeAction = true;
        if (desiredPosition == 0) return;
        if (!canChangeAction) return;
        if (progress == 1f)
        {
            if (currentAction < items.Count - 1)
            {
                items[currentAction].onEndActionTrigger.Invoke();
                currentAction++;
                progress = 0f;
            }
            else if (currentAction == items.Count - 1 && !completedAllActions)
            {
                completedAllActions = true;
                items[currentAction].onEndActionTrigger.Invoke();
            }
            canChangeAction = false;
        }
        else if (progress == 0f)
        {
            items[currentAction].onStartActionTrigger.Invoke();
            if (currentAction > 0)
            {
                currentAction--;
                progress = 1f;
            }
            completedAllActions = false;
            canChangeAction = false;
        }
    }

    public void CheckForChangeAction(Vector3 cursorPosition)
    {
        desiredPosition = GetVectorAxis(cursorPosition);
        progress += desiredPosition * items[currentAction].interactionSpeed;
        progress = Mathf.Clamp01(progress);

        foreach (var tween in items[currentAction].tweens)
        {
            if (desiredPosition != 0f)
                tween.currentItem.SetPositionAndRotation(Vector3.Lerp(tween.transformStartPosition.position, tween.transformEndPosition.position, progress), 
                    Quaternion.Lerp(tween.transformStartPosition.rotation, tween.transformEndPosition.rotation, progress));

            TryToChangeAction();
        }
    }

    public void ResetActions()
    {
        progress = 0f;
        currentAction = 0;
        foreach (var tween in items[0].tweens)
        {
            tween.ResetTween();
        }
        completedAllActions = false;
    }

    public void PlayAnimations(float time)
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }
        playCoroutine = StartCoroutine(LerpEnumerator.OnUpdate(
            (t) => PlayAnimationsUpdate(t), () => items[^1]?.onEndActionTrigger.Invoke(), time));
    }

    public void PlayAnimationsBackwards(float time)
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }
        playCoroutine = StartCoroutine(LerpEnumerator.OnUpdate(
            (t) => PlayAnimationsBackwardsUpdate(t), () => items[^1]?.onStartActionTrigger.Invoke(), time));
    }

    void PlayAnimationsUpdate(float t)
    {
        foreach (var items in items)
            foreach (var tween in items.tweens)
                tween.currentItem.SetPositionAndRotation(
                    Vector3.Lerp(tween.transformStartPosition.position, tween.transformEndPosition.position, t),
                    Quaternion.Lerp(tween.transformStartPosition.rotation, tween.transformEndPosition.rotation, t));
    }

    void PlayAnimationsBackwardsUpdate(float time)
    {
        for (var i = items.Count - 1; i >= 0; i--)
        {
            var tweens = items[i].tweens;
            for (var t = tweens.Count - 1; t >= 0; t--)
            {
                var tween = tweens[t];
                tween.currentItem.SetPositionAndRotation
                    (Vector3.Lerp(tween.transformEndPosition.position, tween.transformStartPosition.position, time),
                    Quaternion.Lerp(tween.transformEndPosition.rotation, tween.transformStartPosition.rotation, time));
            }
        }
    }

    public void PlaySound(AudioClip sound)
    {
        AudioSource.PlayClipAtPoint(sound, transform.position);
    }

    public void Testdsasd(SoundData sound)
    {
        SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).Play(sound);
    }
}

