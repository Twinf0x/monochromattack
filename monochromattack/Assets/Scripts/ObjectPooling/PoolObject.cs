using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolObject
{
    public string name;
    public GameColor gameColor;
    public bool shouldExpand;
    public int initialCount;
    public GameObject prefab;
}