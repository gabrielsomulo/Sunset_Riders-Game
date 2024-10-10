using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolerPlayer : MonoBehaviour
{
    public static BulletPoolerPlayer SharedInstance;
    private List<GameObject> pooledObjects;

    [SerializeField] private GameObject objectToPool;

    private readonly int amountToPool = 15;

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
            obj.transform.SetParent(this.transform);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
