﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [HideInInspector]
    public Vector3 direction;
    [HideInInspector]
    public float speed;

    private void Update()
    {
        transform.position += (direction.normalized * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        DestroySelf();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        DestroySelf();
    }

    public void DestroySelf()
    {
        if(GetComponent<IsPooledObject>() != null)
        {
            gameObject.SetActive(false);
        }  
        else
        {
            Destroy(gameObject);
        }
    }
}
