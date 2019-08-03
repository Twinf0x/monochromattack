using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour, Movement
{
    public float speed = 2f;
    public float duration = 0.5f;

    private Rigidbody2D body;

    private void OnEnable()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public IEnumerator Move(Action callback)
    {
        var direction = EnemyController.instance.GetRandomPointInArena() - transform.position;
        direction = direction.normalized;

        body.velocity = direction * speed;

        yield return new WaitForSeconds(duration);

        body.velocity = Vector2.zero;
        callback.Invoke();
    }
}
