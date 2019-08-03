using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 20;
    public Transform weapon;
    public Transform firePoint;

    private float currentAngle = 0;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Player")
        {
            Activate(other.gameObject);
        }
    }

    private void Activate(GameObject playerObject)
    {
        TimeController.instance.StartAimSlowMotion();
        Destroy(playerObject);
        StartCoroutine(Aim());
    }

    private IEnumerator Aim(float duration = 1f)
    {
        float timer = 0;

        Vector3 mousePosition = Vector3.zero;

        while(timer < duration)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RotateTurret(mousePosition);

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Fire(mousePosition);
    }

    private void RotateTurret(Vector3 targetPosition)
    {
        var xDiff = targetPosition.x - transform.position.x;
        var yDiff = targetPosition.y - transform.position.y;
        float radialAngle = Mathf.Atan2(xDiff, yDiff);
        float degreeAngle = (180 / Mathf.PI) * radialAngle * -1;

        //as weapon is facing to the left by default, subtract 90
        degreeAngle -= 90;

        weapon.RotateAround(transform.position, new Vector3(0, 0, 1), degreeAngle - currentAngle);
        currentAngle = degreeAngle;
    }

    private void Fire(Vector3 targetPosition)
    {
        TimeController.instance.EndAimSlowMotion();

        var xDiff = targetPosition.x - firePoint.position.x;
        var yDiff = targetPosition.y - firePoint.position.y;
        var direction = new Vector2(xDiff, yDiff);

        var projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        var projectileBody = projectile.GetComponent<Rigidbody2D>();
        projectileBody.velocity = direction * projectileSpeed;
    }
}
