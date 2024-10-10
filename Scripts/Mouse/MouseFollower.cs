using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    private AllManager allManager;
    private Vector3 pos, worldPosition;
    private GameObject player;

    private Vector3 posEnemy;
    public Vector3 PosEnemy
    {
        set {posEnemy = value;}
    }

    private bool isAim;
    public bool IsAim
    {
        set {isAim = value;}
    }

    private void Start()
    {
        allManager = FindObjectOfType<AllManager>();
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {   
        if (player != null && allManager.Play)
        {
            if (!isAim)
            {
                pos = Input.mousePosition;
                pos.z = Camera.main.WorldToScreenPoint(player.transform.position).z;
                worldPosition = Camera.main.ScreenToWorldPoint(pos);
                transform.position = new Vector3(player.transform.position.x,worldPosition.y, worldPosition.z);
            }
            else
            {
                transform.position = posEnemy + new Vector3(0,1f,0);
            }
            
        }
    }
}

