using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float abilityProbability = 0.5f;
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
        destructable.onDeath += () => { if(currentAction != null) { StopCoroutine(currentAction); } };
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
}
