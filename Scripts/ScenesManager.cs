using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesManager : MonoBehaviour
{
    private int difficulty;
    public static ScenesManager Instance;

    public int Difficulty 
    {
        get { return difficulty; }
        set { difficulty = value; }
    }

    private void Awake() {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
