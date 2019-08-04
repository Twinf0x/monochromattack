using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnTouch : MonoBehaviour
{
    public bool isActive = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isActive)
        {
            return;
        }

        if(other.gameObject.tag == "Player")
        {
            var destructable = other.gameObject.GetComponent<Destructable>();
            if(destructable.IsIndestructable)
            {
                return;
            }

            var bulletController = other.gameObject.GetComponent<BulletController>();
            bulletController.GameOver(this.gameObject);
        }
    }
}
