using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructor : MonoBehaviour
{
    public float lifeTime = 2f;
    private float timer = 0f;

    private void Update()
    {
        if(timer > lifeTime)
        {
            Destroy(this.gameObject);
        }

        timer += Time.deltaTime;
    }
}
