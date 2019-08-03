using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    public int enemiesInFirstWave = 4;
    public float minEnemyToPlayerDistance = 5f;
    public float maxEnemyToPlayerDistance = 14f;
    public float minTimeBetweenSpawns = 2f;
    public float maxTimeBetweenSpawns = 3f;

    private Coroutine spawnCoroutine;
    private Dictionary<GameColor, ObjectPool> enemyPoolsByColor;
    private List<GameObject> currentEnemies;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        int counter = 0;
        enemyPoolsByColor = new Dictionary<GameColor, ObjectPool>();
        currentEnemies = new List<GameObject>();

        while(counter < 6)
        {
            var gameColor = (GameColor) counter++;
            enemyPoolsByColor.Add(gameColor, ObjectPoolManager.instance.GetEnemyPool(gameColor));
        }
    }

    public void StartFight()
    {
        SpawnInitialWave();
        spawnCoroutine = StartCoroutine(Spawn());
    }

    #region Spawning
    private void SpawnEnemy()
    {
        var distance = Mathf.Lerp(minEnemyToPlayerDistance, maxEnemyToPlayerDistance, Random.Range(0, 1));

        var spawnPoint = GetRandomPointInArena();
        var playerObjects = GameObject.FindGameObjectsWithTag("Player");
        if(playerObjects.Length != 0)
        {
            while((playerObjects[0].transform.position - spawnPoint).sqrMagnitude < distance * distance)
            {
                spawnPoint = GetRandomPointInArena();
            }
        }
        
        SpawnEnemy(spawnPoint);
    }

    private void SpawnEnemy(Vector2 spawnPoint)
    {
        var gameColor = ColorController.instance.GetCurrentGameColor();
        var enemy = enemyPoolsByColor[gameColor].GetObject();
        
        enemy.transform.position = spawnPoint;
        currentEnemies.Add(enemy);
        var destructable = enemy.GetComponent<Destructable>();
        destructable.onDeath += () => currentEnemies.Remove(enemy);
    }

    public Vector3 GetRandomPointInArena()
    {
        return new Vector3(Random.Range(-8.5f, 8.5f),Random.Range(-6.5f, 6.5f), 0f);
    }

    private void SpawnInitialWave()
    {
        for(int i = 0; i < enemiesInFirstWave; i++)
        {
            SpawnEnemy();
        }
    }

    private IEnumerator Spawn()
    {
        while(true)
        {
            var timeToNextSpawn = Mathf.Lerp(minTimeBetweenSpawns, maxTimeBetweenSpawns, Random.Range(0, 1));

            yield return new WaitForSeconds(timeToNextSpawn);

            SpawnEnemy();
        }
    }
    #endregion

    #region Color
    public void ChangeColors()
    {
        var allEnemyObjects = Physics2D.OverlapAreaAll(new Vector2(-8.5f, -6.5f), new Vector2(8.5f, 6.5f), LayerMask.GetMask("Enemy", "EnemyProjectile"));
        foreach(var enemyObject in allEnemyObjects)
        {
            var enemy = enemyObject.gameObject.GetComponent<Enemy>();
            if(enemy != null)
            {
                ReplaceEnemy(enemyObject.gameObject);
                continue;
            }

            var bullet = enemyObject.gameObject.GetComponent<EnemyBullet>();
            if(bullet != null)
            {
                var destructable = enemyObject.gameObject.GetComponent<Destructable>();
                if(destructable != null)
                {
                    destructable.Die();
                }
                else
                {
                    Debug.Log("Bullet didnt have a destructable");
                }
            }
        }
    }

    private void ReplaceEnemy(GameObject enemyObject)
    {
        var position = enemyObject.transform.position;
        var destructable = enemyObject.GetComponent<Destructable>();
        if(destructable != null)
        {
            destructable.Die();
        }

        SpawnEnemy(position);
    }
    #endregion
}
