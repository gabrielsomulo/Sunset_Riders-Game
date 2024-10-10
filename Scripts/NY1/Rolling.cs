using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingWheel : MonoBehaviour
{
    private WavesManager wavesManager;

    [SerializeField]  private bool LeftRoll;

    private void Start()
    {
        wavesManager = FindObjectOfType<WavesManager>();
    }
    
    private void Update()
    {
        if (wavesManager.Wave == true)
        {
            if (LeftRoll)
            {
                transform.Rotate(new Vector3(1,0,0) * 50 * Time.deltaTime, Space.World);
            }
            else
            {
                transform.Rotate(new Vector3(-1,0,0) * 50 * Time.deltaTime, Space.World);
            }
            if (transform.rotation.x > 360 || transform.rotation.x < -360)
            {
                transform.Rotate(new Vector3(0,0,0), Space.World);
            }
        }
        
    }
}
