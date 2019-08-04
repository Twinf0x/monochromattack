using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

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

    public List<PoolObject> enemies;
    public List<PoolObject> enemyProjectiles;

    private List<ObjectPool> enemyPools = new List<ObjectPool>();
    private List<ObjectPool> enemyProjectilePools = new List<ObjectPool>();

    public ObjectPool GetEnemyPool(GameColor gameColor)
    {
        var existingPools = enemyPools.Where(pool => pool.gameColor == gameColor);
        if(existingPools.Any())
        {
            return existingPools.First();
        }

        var matchingPrefabs = enemies.Where(enemy => enemy.gameColor == gameColor);
        if(!matchingPrefabs.Any())
        {
            Debug.LogWarning($"No enemy prefab for game color {gameColor}");
            return null;
        }

        var createdPool = SetupPool(gameColor, matchingPrefabs.FirstOrDefault());
        enemyPools.Add(createdPool);
        return createdPool;
    }

    public ObjectPool GetEnemyProjectilePool(GameColor gameColor)
    {
        var existingPools = enemyProjectilePools.Where(pool => pool.gameColor == gameColor);
        if(existingPools.Any())
        {
            return existingPools.First();
        }

        var matchingPrefabs = enemyProjectiles.Where(projectile => projectile.gameColor == gameColor);
        if(!matchingPrefabs.Any())
        {
            Debug.LogWarning($"No enemy prefab for game color {gameColor}");
            return null;
        }

        var createdPool = SetupPool(gameColor, matchingPrefabs.FirstOrDefault());
        enemyProjectilePools.Add(createdPool);
        return createdPool;
    }

    private ObjectPool SetupPool(GameColor gameColor, PoolObject poolObject)
    {
        GameObject obj = new GameObject(poolObject.name + "Pool");
        ObjectPool pool = obj.AddComponent<ObjectPool>();
        pool.Initialize(gameColor, poolObject);

        return pool;
    }
}
