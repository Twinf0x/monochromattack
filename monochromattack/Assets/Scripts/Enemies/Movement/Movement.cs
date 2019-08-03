using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Movement
{
    IEnumerator Move(Action callback);
}
