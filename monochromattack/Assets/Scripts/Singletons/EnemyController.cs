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
            Initialize();
        }
    }

    private void Initialize()
    {
        int counter = 0;
        enemyPoolsByColor = new Dictionary<GameColor, ObjectPool>();

        while(counter < 6)
        {
            var gameColor = (GameColor) counter;
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
        var gameColor = ColorController.instance.GetCurrentGameColor();
        var enemyPrefab = enemyPoolsByColor[gameColor].GetObject();

        var distance = Mathf.Lerp(minEnemyToPlayerDistance, maxEnemyToPlayerDistance, Random.Range(0, 1));

        var spawnPoint = GetRandomPointInArena();

        while((GameObject.FindGameObjectsWithTag("Player")[0].transform.position - spawnPoint).sqrMagnitude < distance * distance)
        {
            spawnPoint = GetRandomPointInArena();
        }

        Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
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
    public void ChangeColors(GameColor gameColor)
    {

    }
    #endregion
}
