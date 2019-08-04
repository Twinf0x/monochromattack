using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : MonoBehaviour, Ability
{
    public Transform firePoint;
    public int bulletAmount = 3;
    public float bulletSpread = 45f;
    public float bulletSpeed = 3f;
    public float duration = 1f;
    public Enemy enemy;

    private float currentAngle = 0f;

    public IEnumerator Execute(Action callback)
    {
        var objectPool = ObjectPoolManager.instance.GetEnemyProjectilePool(GameColor.Purple);

        var playerObjects = GameObject.FindGameObjectsWithTag("Player");
        var playerDirection = Vector3.zero;

        if(playerObjects.Length != 0)
        {
            playerDirection = playerObjects[0].transform.position - firePoint.position;
        }
        else
        {
            playerDirection = UnityEngine.Random.insideUnitSphere + firePoint.position;
            playerDirection.z = 0;
        }
        enemy.FaceTargetPosition(playerDirection + transform.position);

        RotateFirePoint(playerDirection);

        enemy.SetAnimatorTrigger("Attack");

        var degreesPerStep = bulletSpread / bulletAmount;
        var offset = bulletSpread / 2;

        for(int i = 0; i < bulletAmount; i++)
        {
            var bullet = objectPool.GetObject();
            bullet.transform.position = firePoint.position;
            
            var bulletComponent = bullet.GetComponent<EnemyBullet>();
            var bulletDirection = Quaternion.AngleAxis(offset, Vector3.forward) * playerDirection;
            offset -= degreesPerStep;

            bulletComponent.direction = bulletDirection;
            bulletComponent.speed = bulletSpeed;
        }

        yield return new WaitForSeconds(duration);

        callback.Invoke();
    }

    private void RotateFirePoint(Vector3 targetPosition)
    {
        var xDiff = targetPosition.x - transform.position.x;
        var yDiff = targetPosition.y - transform.position.y;
        float radialAngle = Mathf.Atan2(xDiff, yDiff);
        float degreeAngle = (180 / Mathf.PI) * radialAngle * -1;

        //as weapon is facing to the left by default, subtract 90
        degreeAngle -= 90;

        firePoint.RotateAround(transform.position, new Vector3(0, 0, 1), degreeAngle - currentAngle);
        currentAngle = degreeAngle;
    }
}
