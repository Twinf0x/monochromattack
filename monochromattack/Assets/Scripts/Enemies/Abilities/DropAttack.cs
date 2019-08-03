using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAttack : MonoBehaviour, Ability
{
    public float dropTime = 0.5f;
    public float linearSpeed = 3f;
    public float arcHeight = 5f;
    public AnimationCurve arc;
    public GameObject shockWavePrefab;
    public Transform dropShadow;

    public IEnumerator Execute(Action callback)
    {
        var killer = GetComponent<KillPlayerOnTouch>();
        
        var startPosition = transform.position;
        var targetPosition = Vector3.zero;
        var playerObjects = GameObject.FindGameObjectsWithTag("Player");
        if(playerObjects.Length != 0)
        {
            targetPosition = playerObjects[0].transform.position;
            targetPosition += (Vector3)UnityEngine.Random.insideUnitCircle;
        }
        else
        {
            targetPosition = EnemyController.instance.GetRandomPointInArena();
        }

        killer.isActive = false;

        var flyTime = Vector3.Distance(startPosition, targetPosition) / linearSpeed;
        var timer = 0f;
        
        while(timer <= flyTime)
        {
            var position = Vector3.Lerp(startPosition, targetPosition, (timer/flyTime));
            dropShadow.position = position;
            position.y += arc.Evaluate(timer/flyTime) * arcHeight;
            transform.position = position;

            transform.position = position;
            timer += Time.deltaTime;

            yield return null;
        }

        timer = 0f;
        var dropPosition = transform.position;

        while(timer < dropTime)
        {
            var height = Mathf.Lerp(arcHeight, 0f, timer/dropTime);
            dropPosition.y = height + targetPosition.y;

            transform.position = dropPosition;
            dropShadow.position = targetPosition;

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        killer.isActive = true;
        CreateShockWave(targetPosition);

        callback.Invoke();
    }

    private void CreateShockWave(Vector3 targetPosition)
    {
        Instantiate(shockWavePrefab, targetPosition, Quaternion.identity);
    }

    private void OnDisable()
    {
        dropShadow.position = transform.position;
    }
}
