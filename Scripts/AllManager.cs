using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllManager : MonoBehaviour
{

    private bool isGameOver = false;
    public bool IsGameOver
    {
        get { return isGameOver; }
        set { isGameOver = value; }
    }

    private bool isWin = false;
    public bool IsWin
    {
        get { return isWin; }
        set { isWin = value; }
    }

    private bool play = true;
    public bool Play
    {
        get { return play; }
        set { play = value; }
    }

    private int difficulty = 2;
    public int Difficulty
    {
        get { return difficulty; }
    }

    private int killCounter = 0;
    public int KillCounter
    {
        get { return killCounter; }
        set { killCounter = value; }
    }


    private void Awake()
    {
        Time.timeScale = 1;
        difficulty = ScenesManager.Instance.Difficulty;
    }

    private void LateUpdate()
    {
        if (isGameOver || isWin) 
        {
            play = false;
            Time.timeScale = 0;
        }
    }
}
