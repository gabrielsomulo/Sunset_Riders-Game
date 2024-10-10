using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{
    private WavesManager wavesManager;

    private void Start()
    {
        wavesManager = FindObjectOfType<WavesManager>();
    }

    private void Update()
    {
            if (wavesManager.Wave == true)
            {
            transform.Translate(new Vector3(0,0,1) * 10 * Time.deltaTime, Space.World);
            }
            if (transform.position.z > 110)
            {
                wavesManager.Wave = false;
                gameObject.SetActive(false);
            }
        
    }
}
