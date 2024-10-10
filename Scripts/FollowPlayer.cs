using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private void LateUpdate()
    {
       
        transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z);
        
    }
}
