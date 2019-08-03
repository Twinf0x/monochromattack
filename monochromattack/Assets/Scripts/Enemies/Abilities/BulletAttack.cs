using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : MonoBehaviour, Ability
{
    public int bulletAmount = 3;
    public float bulletSpread = 45f;
    public float duration = 1f;

    public IEnumerator Execute(Action callback)
    {
        var objectPool = ObjectPoolManager.instance.GetEnemyProjectilePool(GameColor.Purple);

        //Get all bullets from pool
        //Get top or bottom attack vector (direction to player +- bulletSpread/2)
        //Set bullet direction via lerp between top/bottom vector (attackVector +- bulletSpread)
        
        yield return new WaitForSeconds(duration);

        callback.Invoke();
    }
}
