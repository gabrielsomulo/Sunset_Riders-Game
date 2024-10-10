using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button menubtn;
    [SerializeField] private Button goBackbtn;
    [SerializeField] private Button endGamebtn;
    [SerializeField] private Button restartbtn;
    [SerializeField] private TextMeshProUGUI endGame;
    
    private AllManager allManager;

    private void Start()
    {
        allManager = FindObjectOfType<AllManager>();
    }

    private void Update()
    {
        if (allManager.IsGameOver == true || allManager.IsWin == true)
        {
            endGamebtn.gameObject.SetActive(true);

            if(allManager.IsGameOver == true){
                endGamebtn.gameObject.GetComponent<Image>().color = Color.red;
                endGame.text = "Game Over";
            }
            else if (allManager.IsWin == true)
            {
                endGamebtn.gameObject.GetComponent<Image>().color = Color.green;
                endGame.text = "Winner";
            }

            restartbtn.gameObject.SetActive(true);
        }
    }

    public void Scene(int s){
        allManager.Play = true;
        Time.timeScale = 1;
        SceneManager.LoadScene(s);
    }

    public void Settbtn()
    {
        if(allManager.IsGameOver == false && allManager.IsWin == false)
        {
            menubtn.gameObject.SetActive(true);
            goBackbtn.gameObject.SetActive(true);
            Time.timeScale = 0;
            allManager.Play = false;
        }
        
    }

    public void GoBackbtn()
    {
        Time.timeScale = 1;
        allManager.Play = true;
        menubtn.gameObject.SetActive(false);
        goBackbtn.gameObject.SetActive(false);
    }
}
