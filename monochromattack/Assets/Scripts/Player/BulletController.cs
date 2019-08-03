using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float movementSpeed = 5;
    public float dashSpeed = 30;
    public float dashDrag = 7;
    private Vector2 direction;
    private bool isDashing = false;
    private Rigidbody2D body;
    private Destructable destructable;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        destructable = GetComponent<Destructable>();
    }

    private void Update()
    {
        if(isDashing)
        {
            return;
        }

        direction = GetMovementDirection();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Dash(direction);
        }
        else
        {
            Move(direction);
        }
    }

    private Vector2 GetMovementDirection()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        return new Vector2(x, y).normalized;
    }

    private void Move(Vector2 direction)
    {
        body.velocity = direction * movementSpeed;
    }

    private void Dash(Vector2 direction)
    {
        StartCoroutine(PerformDash(direction));
    }

    private IEnumerator PerformDash(Vector2 direction, float duration = 0.5f)
    {
        isDashing = true;
        float timer = 0;

        body.velocity = direction * dashSpeed;
        body.drag = dashDrag;
        destructable.IsIndestructable = true;

        while(timer < duration)
        {
            body.drag = Mathf.Lerp(dashDrag, 0f, (timer / duration));

            if(body.velocity.magnitude < movementSpeed && isDashing)
            {
                isDashing = false;
            }
            
            timer += Time.deltaTime;
            yield return null;
        }

        destructable.IsIndestructable = false;

        isDashing = false;
    }
}
