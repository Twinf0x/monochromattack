using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    private bool isIndestructable = false;
    public bool IsIndestructable
    {
        get{ return isIndestructable; }
        set{ isIndestructable = value; }
    }

    public Action onDeath = delegate { };

    public void Die()
    {
        if(isIndestructable)
        {
            return;
        }

        //Spawn corpses and stuff
        onDeath();

        Destroy(this.gameObject);
    }
}
