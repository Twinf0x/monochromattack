using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarnAsTurret : MonoBehaviour, Ability
{
    public GameObject normalSprite;
    public GameObject turretSprite;
    public float tarnDuration = 1f;
    public float waitDuration = 0.5f;

    public IEnumerator Execute (Action callback)
    {
        normalSprite.SetActive(false);
        turretSprite.SetActive(true);

        yield return new WaitForSeconds(tarnDuration);

        normalSprite.SetActive(true);
        turretSprite.SetActive(false);

        yield return new WaitForSeconds(waitDuration);

        callback.Invoke();
    }
}
