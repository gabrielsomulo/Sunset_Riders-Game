using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageLogic : MonoBehaviour
{
    private int life = 15;
    private bool isShot = false, visible = false;

    private readonly Color newColor = Color.red;

    private List<bool> cursorStates;

    private Color[] originalColors;
    private Material[] materials;
    private AllManager allManager;
    private WavesManager wavesManager;
    private Bounds bounds;
    private Camera cam;
    private Renderer objRenderer;
    private Vector3 mouseScreenPosition, mouseWorldPosition;
    private Collider[] cageColliders;
    private MouseFollower mouseFollower;
    private GameObject player;



    private void Start() 
    {
        allManager = FindObjectOfType<AllManager>();
        wavesManager = FindObjectOfType<WavesManager>();
        mouseFollower = FindObjectOfType<MouseFollower>();

        cageColliders = GetComponentsInChildren<Collider>();

        cursorStates = new List<bool>();

        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            materials = objRenderer.materials;
            originalColors = new Color[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                originalColors[i] = materials[i].color;
            }
        }

        player = GameObject.Find("PlayerController");
        cam = Camera.main;
    }

    private void Update() 
    {
        visible = IsVisible(cam, gameObject);

        if (allManager.Play && wavesManager.Wave && visible)
        {
            mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
                
            foreach (Collider cageCollider in cageColliders)
            {
                bounds = cageCollider.bounds;
                bounds.Expand(2f);
                    
                if (bounds.Contains(mouseWorldPosition)) 
                {   
                    cursorStates.Add(true);
                    mouseFollower.PosEnemy = transform.position;
                    mouseFollower.IsAim = true;
                }
                else
                {
                    cursorStates.Add(false);
                }

                if (cursorStates.Count == 1 && !cursorStates[0])
                {
                    cursorStates.Clear();
                }
                else if (cursorStates.Count == 2)
                {
                    if (!cursorStates[1])
                    {
                        mouseFollower.IsAim = false;
                        cursorStates.Clear();
                    }
                    else if (cursorStates[1])
                    {
                        cursorStates.Clear();
                        cursorStates.Add(true);
                    }
                } 
            }

            if (life <= 0)
            {
                GameObject pooledProjectile = SeizedPooler.SharedInstance.GetPooledObject();
                if (pooledProjectile != null)
                {
                    pooledProjectile.SetActive(true);
                    pooledProjectile.transform.position = player.transform.position + new Vector3(0,0,2);
                }

                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BulletPlayer"))
        {
            if(!isShot)
            {
                isShot = true;
                StartCoroutine(Damage());
            }
        }
    }

    private bool IsVisible (Camera c, GameObject objectOnCam)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = objectOnCam.transform.position;

        foreach (var plane in planes)
        {
            if(plane.GetDistanceToPoint(point) < -0.5)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator Damage()
    {
        life--;
        SetColors(newColor);
        yield return new WaitForSeconds(0.25f);
        ResetColors();
        
        isShot = false;
    }

    private void SetColors(Color color)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = color;
        }
    }

    private void ResetColors()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
    }
}