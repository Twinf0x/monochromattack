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
    [SerializeField] private List<GameObject> killExplosionPrefabs;
    
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
        //degreeAngle -= 90;

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

        var spawnPoint = EnemyController.instance.GetRandomPointInArena();

        while((transform.position - spawnPoint).sqrMagnitude < distance * distance)
        {
            spawnPoint = EnemyController.instance.GetRandomPointInArena();
        }

        Instantiate(turretPrefab, spawnPoint, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            var destructable = other.gameObject.GetComponent<Destructable>();
            if(destructable != null && !destructable.IsIndestructable)
            {
                foreach (GameObject prefab in killExplosionPrefabs) {
                    Instantiate(prefab, other.transform.position, Quaternion.identity);
                }
                destructable.Die();
                HighscoreController.instance.IncreaseScore();
            }

            var enemyProjectile = other.gameObject.GetComponent<EnemyBullet>();
            if(enemyProjectile != null)
            {
                enemyProjectile.DestroySelf();
            }
        }
    }
}
