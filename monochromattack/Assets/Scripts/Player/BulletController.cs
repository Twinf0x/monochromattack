using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float movementSpeed = 5;
    public float dashSpeed = 30;
    public float dashDrag = 7;
    public float dashCoolDown = 1f;
    public SpriteRenderer sprite;
    public Animator animator;
    public SpriteAfterImage afterImage;
    public GameObject dashRefillParticlePrefab;
    private Vector2 direction;
    private bool isDashing = false;
    private Rigidbody2D body;
    private Destructable destructable;
    private bool isFacingRight = true;
    private float timeSinceLastDash;
    private bool canDash = true;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        destructable = GetComponent<Destructable>();
        //destructable.onDeath += () => this.GameOver();
        ColorController.instance.AddSprite(sprite);
        afterImage.enabled = false;
        timeSinceLastDash = dashCoolDown;
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
        if(Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            Dash(direction);
        }
        else
        {
            Move(direction);
            if(!canDash)
            {
                timeSinceLastDash += Time.deltaTime;
                if(timeSinceLastDash >= dashCoolDown)
                {
                    canDash = true;
                    AudioManager.instance.Play("DashReady");
                    Instantiate(dashRefillParticlePrefab, transform.position, Quaternion.identity);
                }
            }
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
            AudioManager.instance.Stop("PlayerWalk");
        }
        else
        {
            animator.SetBool("isWalking", true);
            AudioManager.instance.Play("PlayerWalk");
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
        timeSinceLastDash = 0f;
        canDash = false;
        StartCoroutine(PerformDash(direction));
    }

    private IEnumerator PerformDash(Vector2 direction, float duration = 0.5f)
    {
        isDashing = true;
        AudioManager.instance.Play("PlayerDash");
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

    public void GameOver(GameObject killer)
    {
        TimeController.instance.SetGameOver();
        AudioManager.instance.Stop("Fight");
        AudioManager.instance.Play("Chill");
        AudioManager.instance.Play("Scratch");

        EnemyController.instance.DeactivateAllBut(killer);
        GameObject.FindGameObjectsWithTag("Arena")[0].SetActive(false);
        StartCoroutine(ZoomInOnKiller(killer));
    }

    private IEnumerator ZoomInOnKiller(GameObject killer, float duration = 1f)
    {
        var timer = 0f;
        var startPosition = Camera.main.transform.position;
        var targetPosition = killer.transform.position + (Vector3.right * 2f);

        while(timer < duration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(8, 3, (timer/duration));
            var temp = Vector3.Lerp(startPosition, targetPosition, (timer/duration));
            temp.z = startPosition.z;
            Camera.main.transform.position = temp;

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        HighscoreController.instance.ToFinalScore();
    }
}
