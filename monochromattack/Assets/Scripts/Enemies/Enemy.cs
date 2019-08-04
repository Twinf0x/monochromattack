using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float abilityProbability = 0.5f;
    public SpriteRenderer sprite;
    public bool isFacingRight = false;
    public Animator animator;
    private Movement movement;
    private Ability ability;
    private bool isBusy = false;
    private Coroutine currentAction;

    private void OnEnable() 
    {
        Initialize();
    }

    private void Initialize()
    {
        isBusy = false;
        movement = GetComponent<Movement>();
        ability = GetComponent<Ability>();
        var destructable = GetComponent<Destructable>();
        destructable.onDeath += () => { 
            if(currentAction != null) 
            { 
                StopCoroutine(currentAction); 
            } 
            AudioManager.instance.Play("EnemyDies"); 
        };
    }

    private void Update()
    {
        if(isBusy)
        {
            return;
        }

        isBusy = true;
        if(Random.Range(0f, 1f) > abilityProbability)
        {
            StartCoroutine(movement.Move(() => isBusy = false));
        }
        else
        {
            StartCoroutine(ability.Execute(() => isBusy = false));
        }
    }

    public void FaceTargetPosition(Vector3 position)
    {
        if(position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }

        if(position.x < transform.position.x && isFacingRight)
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

    public void SetAnimatorBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    public void SetAnimatorTrigger(string name)
    {
        animator.SetTrigger(name);
    }
}
