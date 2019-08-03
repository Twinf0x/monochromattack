using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float abilityProbability = 0.5f;
    private Movement movement;
    private Ability ability;
    private bool isBusy = false;

    private void OnEnable() 
    {
        Initialize();
    }

    private void Initialize()
    {
        movement = GetComponent<Movement>();
        ability = GetComponent<Ability>();
        //Set scoring stuff
    }

    private void Update()
    {
        if(isBusy)
        {
            return;
        }

        isBusy = true;
        if(Random.Range(0, 1) > abilityProbability)
        {
            StartCoroutine(movement.Move(() => isBusy = false));
        }
        else
        {
            StartCoroutine(ability.Execute(() => isBusy = false));
        }
    }
}
