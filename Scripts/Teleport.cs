using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private GameObject TeleportPoint;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            other.transform.position = TeleportPoint.transform.position;
        }
    }
}
