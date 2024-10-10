using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveLogic : MonoBehaviour
{
    private WavesManager wavesManager;

    void Start()
    {
        wavesManager = FindObjectOfType<WavesManager>();
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.CompareTag("Player")){
            wavesManager.Wave = true;
        }
    }
}
