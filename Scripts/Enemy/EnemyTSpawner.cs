using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTSpawner : MonoBehaviour
{
    private AllManager allManager;
    private WavesManager wavesManager;
    private Camera cam;

    private bool visible, isSpawning = false;
    private List<GameObject> enemies;

    private void Start()
    {
       allManager = FindObjectOfType<AllManager>(); 
       wavesManager = FindObjectOfType<WavesManager>(); 

       enemies = new List<GameObject>();

       cam = Camera.main; 
    }

    private void Update()
    {   
        visible = IsVisible(cam, gameObject);
        
        if(visible && allManager.Play && wavesManager.Wave == true)
        {
            if (!isSpawning && (enemies.Count == 0 || (enemies.Count > 0 && !enemies[0].activeSelf)))
            {
                if (enemies.Count > 0)
                {
                    enemies.Clear();
                }
                
                StartCoroutine(SpawnEnemy());
            }

        }
    }

    private bool IsVisible (Camera c, GameObject objectOnCam)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = objectOnCam.transform.position;

        foreach (var plane in planes)
        {
            if(plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnEnemy()
    {
        isSpawning = true; 

        yield return new WaitForSeconds(3);

         GameObject pooledProjectile = EnemyTPooler.SharedInstance.GetPooledObject();
        if (pooledProjectile != null)
        {
            pooledProjectile.SetActive(true);
            pooledProjectile.transform.rotation = transform.rotation;
            pooledProjectile.transform.position = transform.position;
            pooledProjectile.GetComponent<EnemyDefaultT>().IsWalking = true;
            enemies.Add(pooledProjectile);
        }

        isSpawning = false;
    }
}
