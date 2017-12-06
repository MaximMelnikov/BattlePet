using System;
using System.Collections;
using UnityEngine;

public class MonoBehaviourEx : MonoBehaviour {
    internal Coroutine Wait(float seconds, Action onEnd)
    {
        return StartCoroutine(WaitCoroutine(seconds, onEnd));
    }
    IEnumerator WaitCoroutine(float seconds, Action onEnd)
    {
        yield return new WaitForSeconds(seconds);
        if (onEnd != null)
        {
            onEnd.Invoke();
        }
    }

    internal Coroutine WaitFrameEnd(Action onEnd)
    {
        return StartCoroutine(WaitFrameEndCoroutine(onEnd));
    }
    IEnumerator WaitFrameEndCoroutine(Action onEnd)
    {
        yield return new WaitForEndOfFrame();
        if (onEnd != null)
        {
            onEnd.Invoke();
        }
    }
}
