using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttack : MonoBehaviour, Ability
{
    public LineRenderer indicator;
    public Transform firePoint;
    public float chargeSpeed = 6;
    public float windUpDuration = 1.5f;
    public float coolDownDuration = 1f;
    public GameObject shockWavePrefab;
    public Enemy enemy;

    private Action collisionCallback = delegate { };
    private float currentAngle = 0f;
    private bool isCharging = false;
    private Vector3 playerDirection;
    private Rigidbody2D body;

    public IEnumerator Execute(Action callback)
    {
        collisionCallback = callback;

        var playerObjects = GameObject.FindGameObjectsWithTag("Player");
        playerDirection = Vector3.zero;

        if(playerObjects.Length != 0)
        {
            playerDirection = playerObjects[0].transform.position - firePoint.position;
        }
        else
        {
            playerDirection = UnityEngine.Random.insideUnitSphere + firePoint.position;
            playerDirection.z = 0;
        }
        playerDirection = playerDirection.normalized;
        enemy.FaceTargetPosition(playerDirection + transform.position);

        RotateFirePoint(playerDirection);

        indicator.SetPositions(new Vector3[]{ firePoint.position, firePoint.position + (playerDirection * 500) });
        indicator.widthMultiplier = 0.5f;
        indicator.gameObject.SetActive(true);

        var timer = 0f;
        while(timer < windUpDuration)
        {
            indicator.widthMultiplier = 0.5f - ((timer / windUpDuration) / 2);
            timer += Time.deltaTime;
            yield return null;
        }

        indicator.gameObject.SetActive(false);
        isCharging = true;

        body = GetComponent<Rigidbody2D>();
        while(isCharging)
        {
            body.velocity = playerDirection * chargeSpeed;
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag != "Wall")
        {
            return;
        }

        isCharging = false;
        body.velocity = Vector2.zero;
        Instantiate(shockWavePrefab, transform.position + (playerDirection * 0.5f), Quaternion.identity);
        StartCoroutine(CoolDown());
    }

    private IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDownDuration);
        collisionCallback.Invoke();
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
