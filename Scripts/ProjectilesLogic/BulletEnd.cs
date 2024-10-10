using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnd : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
      cam = Camera.main;
    }

    private void Update()
    {
      if (transform.position.x > 5 || transform.position.x < -15 || IsVisible(cam, this.gameObject) == false)
      {
        Destroy();
      } 
    }

    private void Destroy()
    {
      gameObject.SetActive(false);
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

    private void OnTriggerEnter(Collider other)
    {
      if(other.gameObject.CompareTag("BulletEnemy") || other.gameObject.CompareTag("BulletPlayer") || other.gameObject.CompareTag("AreaDamage") || other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("BulletDestroyer"))
      {
        Destroy();
      }

      if (other.gameObject.CompareTag("Player") && gameObject.CompareTag("BulletEnemy"))
      {
        Destroy();
      }

      if (( other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyDefender") ) && gameObject.CompareTag("BulletPlayer"))
      {
        Destroy();
      }
    }
}
