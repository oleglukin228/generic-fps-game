using System;
using System.Collections;
using System.Net;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Transform targetObject;
    public Transform startPosition;
    public Transform endPosition;
    public float timeToMove = 5f;
    Coroutine coroutine;
    bool buttonSwitch;

    public void MoveGameObject()
    {
        if (coroutine != null) return;

        if (!buttonSwitch)
            coroutine = StartCoroutine(
                LerpEnumerator.OnUpdate(
                    (t) => targetObject.position = Vector3.Lerp(startPosition.position, endPosition.position, t),
                    () => OnEndAction(),
                    timeToMove));
        else
            coroutine = StartCoroutine(
                LerpEnumerator.OnUpdate(
                    (t) => targetObject.position = Vector3.Lerp(endPosition.position, startPosition.position, t),
                    () => OnEndAction(),
                    timeToMove));
    }

    void OnEndAction()
    {
        buttonSwitch = !buttonSwitch;
        StopCoroutine(coroutine);
        coroutine = null;
    }
}
