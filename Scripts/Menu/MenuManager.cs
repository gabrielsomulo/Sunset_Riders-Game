using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;


public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button easybtn;
    [SerializeField] private Button normalbtn;
    [SerializeField] private Button hardbtn;

    private void Awake(){
        Difficulty(2);
    }
    public void Exit()
    {
        Application.Quit(); 
    }

    public void Difficulty(int d){
        switch(d){
            case 1:
                easybtn.interactable = false;
                normalbtn.interactable = true;
                hardbtn.interactable = true;
            break;
            case 2:
                easybtn.interactable = true;
                normalbtn.interactable = false;
                hardbtn.interactable = true;
            break;
            case 3:
                easybtn.interactable = true;
                normalbtn.interactable = true;
                hardbtn.interactable = false;
            break;
        }
        ScenesManager.Instance.Difficulty = d;
    }
    
    public void Scene(int s){
        SceneManager.LoadScene(s);
    }
    
    
}
