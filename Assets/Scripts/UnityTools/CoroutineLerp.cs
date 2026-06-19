using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Contains methods for creating routines which can be used as coroutines to easy lerp over a time period.
/// 
/// Example moves an object between two points over 5 seconds:
/// 
///     Vector3 startPoint = transform.position;
///     Vector3 endPoint = new Vector3(0f, 10f, 0f);
/// 
///     StartCoroutine(
///         LerpEnumerator.OnUpdate(
///             (t) => transform.position = Vector3.Lerp(startPoint, endPoint, t),
///             5f
///         )
///     );
///  
/// </summary>
public static class LerpEnumerator
{
    /// <summary>
    /// Returns a routine that executes `action` on each frame for `duration` (unity scaled time), passing float value from 0-1 linearly across the time period.
    /// </summary>
    /// <param name="action">Action that accepts a float argument (0-1).</param>
    /// <param name="duration">Duration of the routine.</param>
    /// <returns></returns>
    public static IEnumerator OnUpdate(Action<float> action, Action onEndAction, float duration)
    {
        float startTime = Time.time;

        float t;
        while ((t = (Time.time - startTime) / duration) <= 1f)
        {
            action.Invoke(t);
            yield return null;
        }
        action.Invoke(1f);
        onEndAction?.Invoke();
    }

    public static IEnumerator OnUpdate(Action<float> action, float duration)
    {
        float startTime = Time.time;

        float t;
        while ((t = (Time.time - startTime) / duration) <= 1f)
        {
            action.Invoke(t);
            yield return null;
        }
        action.Invoke(1f);
    }

    /// <summary>
    /// Returns a routine that executes `action` on each frame for `duration` (unity unscaled time), passing float value from 0-1 linearly across the time period.
    /// </summary>
    /// <param name="action">Action that accepts a float argument (0-1).</param>
    /// <param name="duration">Duration of the routine.</param>
    /// <returns></returns>
    public static IEnumerator OnUnscaledUpdate(Action<float> action, float duration)
    {
        float startTime = Time.unscaledTime;

        float t;
        while ((t = (Time.unscaledTime - startTime) / duration) <= 1f)
        {
            action.Invoke(t);
            yield return null;
        }
        action.Invoke(1f);
    }

}

/*public class CoroutineLerp
{
    private Coroutine coroutine;
    public delegate void MarkerDelegate(bool b);
    public MarkerDelegate marker;
    public float Progress;

    public void Begin(float duration, MonoBehaviour mono)
    {
        if (coroutine != null)
            mono.StopCoroutine(coroutine);
        coroutine = mono.StartCoroutine(Perform(duration));
    }

    public void Stop(MonoBehaviour mono)
    {
        if (coroutine != null)
            mono.StopCoroutine(coroutine);
    }

    public IEnumerator Perform(float duration)
    {
        if (marker != null)
            marker.Invoke(true);

        Progress = 0.0f;

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            Progress = Mathf.Lerp(0.0f, 1.0f, t);
            yield return null;
        }

        Progress = 1.0f;

        if (marker != null)
            marker.Invoke(false);
    }
}*/