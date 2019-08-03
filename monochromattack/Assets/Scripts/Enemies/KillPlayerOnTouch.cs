using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnTouch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            var destructable = other.gameObject.GetComponent<Destructable>();
            if(destructable.IsIndestructable)
            {
                return;
            }

            destructable.Die();
        }
    }
}
