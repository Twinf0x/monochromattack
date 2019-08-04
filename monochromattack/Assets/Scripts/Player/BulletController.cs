using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float movementSpeed = 5;
    public float dashSpeed = 30;
    public float dashDrag = 7;
    public SpriteRenderer sprite;
    public Animator animator;
    public SpriteAfterImage afterImage;
    private Vector2 direction;
    private bool isDashing = false;
    private Rigidbody2D body;
    private Destructable destructable;
    private bool isFacingRight = true;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        destructable = GetComponent<Destructable>();
        destructable.onDeath += () => this.GameOver();
        ColorController.instance.AddSprite(sprite);
        afterImage.enabled = false;
    }

    private void OnDestroy() 
    {
        ColorController.instance.RemoveSprite(sprite);
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
        if(direction == Vector2.zero)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }

        if(direction.x > 0 && !isFacingRight)
        {
            Flip();
        }

        if(direction.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        var temp = sprite.gameObject.transform.localScale;
        temp.x *= -1;
        sprite.gameObject.transform.localScale = temp;
        isFacingRight = !isFacingRight;
    }

    private void Dash(Vector2 direction)
    {
        StartCoroutine(PerformDash(direction));
    }

    private IEnumerator PerformDash(Vector2 direction, float duration = 0.5f)
    {
        isDashing = true;
        afterImage.enabled = true;
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
        afterImage.enabled = false;
        isDashing = false;
    }

    private void GameOver()
    {
        TimeController.instance.SetGameOver();
        AudioManager.instance.Stop("Fight");
        AudioManager.instance.Play("Chill");
        AudioManager.instance.Play("Scratch");

    }
}
