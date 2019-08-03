using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMovement : MonoBehaviour, Movement
{
    public float duration = 0.5f;

    public IEnumerator Move(Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback.Invoke();
    }
}
