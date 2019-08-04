using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBomb : MonoBehaviour, Ability
{
    public Transform firePoint;
    public float bulletSpeed = 3f;
    public float duration = 2f;
    public float arcHeight = 5f;
    public AnimationCurve arc;
    public GameObject firePatchPrefab;
    public Enemy enemy;

    public IEnumerator Execute(Action callback)
    {
        var objectPool = ObjectPoolManager.instance.GetEnemyProjectilePool(GameColor.Red);
        var bombObject = objectPool.GetObject();

        var startPosition = firePoint.position;
        var targetPosition = Vector3.zero;
        var playerObjects = GameObject.FindGameObjectsWithTag("Player");
        if(playerObjects.Length != 0)
        {
            targetPosition = playerObjects[0].transform.position;
        }
        else
        {
            targetPosition = EnemyController.instance.GetRandomPointInArena();
        }
        enemy.FaceTargetPosition(targetPosition);
        enemy.SetAnimatorTrigger("Attack");
        AudioManager.instance.Play("RedFire");

        var flyTime = Vector3.Distance(startPosition, targetPosition) / bulletSpeed;
        var timer = 0f;
        
        while(timer <= flyTime)
        {
            var position = Vector3.Lerp(startPosition, targetPosition, (timer/flyTime));
            position.y += arc.Evaluate(timer/flyTime) * arcHeight;

            bombObject.transform.position = position;
            timer += Time.deltaTime;

            yield return null;
        }

        var bombDestrcutable = bombObject.GetComponent<Destructable>();
        bombDestrcutable.Die();

        CreateFirePatch(targetPosition);
        callback.Invoke();
    }

    private void CreateFirePatch(Vector3 targetPosition)
    {
        AudioManager.instance.Play("RedImpact");
        Instantiate(firePatchPrefab, targetPosition, Quaternion.identity);
    }
}
