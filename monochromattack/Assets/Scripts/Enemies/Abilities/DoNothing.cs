using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothing : MonoBehaviour, Ability
{
    public IEnumerator Execute(Action callback)
    {
        yield return null;
        callback.Invoke();
    }
}
