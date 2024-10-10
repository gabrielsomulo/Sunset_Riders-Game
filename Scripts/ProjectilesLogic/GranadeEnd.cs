using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeEnd : MonoBehaviour
{
    private readonly Color newColor = Color.red;

    private Color[] originalColors;
    private Material[] materials;
    private Transform[] allTransforms;
    private GameObject areaDamage;
    private Renderer objRenderer;
    private AllManager allManager;
    private Camera cam;

    private bool isDetonated = false;

    private void Start()
    {
        allManager = FindObjectOfType<AllManager>();

        allTransforms = GetComponentsInChildren<Transform>(true);

        foreach (Transform t in allTransforms)
        {
            if (t.CompareTag("AreaDamage"))
            {
                areaDamage = t.gameObject;
                break;
            }
        }

        cam = Camera.main;

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
    }

    private void OnEnable() {
        isDetonated = false;
    }

    private void OnDisable() {
        isDetonated = true;
    }

    private void Update()
    {
      if (!IsVisible(cam, gameObject) && !isDetonated)
      {
        Destroy();
      } 
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
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

    IEnumerator KBoom()
    {
        isDetonated = true;

        gameObject.GetComponent<Collider>().isTrigger = false;

        SetColors(newColor);
        yield return new WaitForSeconds(0.5f);
        ResetColors();
        yield return new WaitForSeconds(0.5f);
        SetColors(newColor);
        yield return new WaitForSeconds(0.5f);
        ResetColors();

        areaDamage.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        areaDamage.SetActive(false);

        gameObject.GetComponent<Collider>().isTrigger = true;
        gameObject.SetActive(false);
        

        isDetonated = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("BulletDestroyer") || other.gameObject.CompareTag("Player"))
        {
            if(allManager.Play && !isDetonated)
            {
                StartCoroutine(KBoom());
            }
        }
    }
}
