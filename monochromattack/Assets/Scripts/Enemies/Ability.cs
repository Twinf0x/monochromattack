using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Ability
{
    IEnumerator Execute(Action callback);
}
