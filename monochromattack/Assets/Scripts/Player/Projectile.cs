using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject turretPrefab;
    public float spawnThreshold = 0.3f;
    public float minTurretToPlayerDistance = 10f;
    public float maxTurretToPlayerDistance = 14f;
    private Rigidbody2D body;
    private float currentAngle = 0f;
    
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        RotateInMovementDirection();

        if(body.velocity.magnitude < spawnThreshold)
        {
            SpawnPlayer();
            SpawnTurret();
        }
    }

    private void RotateInMovementDirection()
    {
        var direction = body.velocity;
        float radialAngle = Mathf.Atan2(direction.x, direction.y);
        float degreeAngle = (180 / Mathf.PI) * radialAngle * -1;

        //as projectile is facing to the left by default, subtract 90
        degreeAngle -= 90;

        transform.RotateAround(transform.position, new Vector3(0, 0, 1), degreeAngle - currentAngle);
        currentAngle = degreeAngle;
    }

    private void SpawnPlayer()
    {
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    private void SpawnTurret()
    {
        var distance = Mathf.Lerp(minTurretToPlayerDistance, maxTurretToPlayerDistance, Random.Range(0, 1));

        var spawnPoint = GetRandomPointInArena();

        while((transform.position - spawnPoint).sqrMagnitude < distance * distance)
        {
            spawnPoint = GetRandomPointInArena();
        }

        Instantiate(turretPrefab, spawnPoint, Quaternion.identity);
    }

    private Vector3 GetRandomPointInArena()
    {
        return new Vector3(Random.Range(-8.5f, 8.5f),Random.Range(-6.5f, 6.5f), 0f);
    }
}
