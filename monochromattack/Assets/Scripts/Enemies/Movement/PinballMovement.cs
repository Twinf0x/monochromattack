using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinballMovement : MonoBehaviour, Movement
{
    public float speed = 4f;
    public Enemy enemy;
    private Vector2 direction;
    private Rigidbody2D body;

    public IEnumerator Move(Action callback)
    {
        var dir = UnityEngine.Random.Range(0, 4);
        direction = Quaternion.AngleAxis((45 + (dir * 90)), Vector3.forward) * Vector3.up;
        body = GetComponent<Rigidbody2D>();

        while(true)
        {
            body.velocity = direction.normalized * speed;
            yield return null;
        }

        callback.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag != "Wall")
        {
            return;
        }

        bool isCollidingTop = Physics2D.OverlapCircleAll((Vector3.up) + transform.position, 0.25f, LayerMask.GetMask("Wall")).Length > 0;
        bool isCollidingBottom = Physics2D.OverlapCircleAll((Vector3.down) + transform.position, 0.25f, LayerMask.GetMask("Wall")).Length > 0;
        bool isCollidingLeft = Physics2D.OverlapCircleAll((Vector3.left) + transform.position, 0.25f, LayerMask.GetMask("Wall")).Length > 0;
        bool isCollidingRight = Physics2D.OverlapCircleAll((Vector3.right) + transform.position, 0.25f, LayerMask.GetMask("Wall")).Length > 0;
        float rotationFactor = 0f;
        
        if(isCollidingTop)
        {
            if(direction.x > 0)
            {
                rotationFactor -= 1;
            }
            else
            {
                rotationFactor += 1;
            }
        }

        if(isCollidingBottom)
        {
            if(direction.x > 0)
            {
                rotationFactor += 1;
            }
            else
            {
                rotationFactor -= 1;
            }
        }

        if(isCollidingLeft)
        {
            if(direction.y > 0)
            {
                rotationFactor -= 1;
            }
            else
            {
                rotationFactor += 1;
            }
        }

        if(isCollidingRight)
        {
            if(direction.y > 0)
            {
                rotationFactor += 1;
            }
            else
            {
                rotationFactor -= 1;
            }
        }

        direction = Quaternion.AngleAxis(rotationFactor * 90, Vector3.forward) * direction;
        enemy.FaceTargetPosition(direction.normalized + (Vector2)transform.position);
    }
}
