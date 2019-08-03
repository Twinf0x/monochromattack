using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameColor gameColor;
    private bool shouldExpand = true;
    private GameObject prefab;
    private List<GameObject> pooledObjects = new List<GameObject>();

    public void Initialize(GameColor gameColor, PoolObject poolObject)
    {
        this.gameColor = gameColor;
        shouldExpand = poolObject.shouldExpand;
        prefab = poolObject.prefab;

        for(int i = 0; i < poolObject.initialCount; i++)
        {
            CreateObject();
        }
    }

    public GameObject GetObject()
    {
        GameObject obj = null;
        var availableObjects = pooledObjects.Where(pooledObj => !pooledObj.activeInHierarchy);
        if(availableObjects.Any())
        {
            obj = availableObjects.Last();
            obj.gameObject.SetActive(true);
        }
        else if(shouldExpand)
        {
            obj = CreateObject();
            obj.gameObject.SetActive(true);
        }
		return obj;
    }

    private GameObject CreateObject()
    {
        var obj = Instantiate(prefab, this.transform);
        obj.SetActive(false);
        pooledObjects.Add(obj);

        return obj;
    }
}
