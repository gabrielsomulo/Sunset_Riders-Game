using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> limiters;

    private bool wave = false;
    public bool Wave 
    {
        get { return wave; }
        set { wave = value; }
    }

    private void Update()
    {
        if (wave == true)
        {
            for (int i = 0; i < limiters.Count; i++)
            {
                limiters[i].SetActive(true);
            }
        }
        else
        {
             for (int i = 0; i < limiters.Count; i++)
            {
                limiters[i].SetActive(false);
            }
        }


    }
}
