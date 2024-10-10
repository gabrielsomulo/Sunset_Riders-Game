using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefaultT : MonoBehaviour
{
    private GameObject target, player, projectileSpawner, weapon;
    private Transform[] allTransforms;
    private Camera cam;
    private Vector3 direction, mouseScreenPosition, mouseWorldPosition;
    private AllManager allManager;
    private MouseFollower mouseFollower;
    private Animator anim;
    private Collider[] enemyColliders;
    private Bounds bounds;
    private Coroutine shootingCoroutine, damageRoutine;
    private Color[] originalColors;
    private Material[] materials;
    private readonly Color newColor = Color.red;
    private Renderer objRenderer;

    private bool visible = false, isShooting = false, isLocked = false, isShot = false;
    private int difficulty, life = 15;

    private List<bool> cursorStates;

    private readonly float shootCooldown = 1.2f, shootInterval = 0.1f;
    private readonly int bulletsToShoot = 10, bulletVelocity = 30;

    private bool isWalking = false;

    public bool IsWalking
    {
        set {isWalking = value;}
    }
    
    private void Start()
    {
        allManager = FindObjectOfType<AllManager>();
        mouseFollower = FindObjectOfType<MouseFollower>();

        allTransforms = GetComponentsInChildren<Transform>(); 
        enemyColliders = GetComponentsInChildren<Collider>();

        difficulty = allManager.Difficulty;

        cursorStates = new List<bool>();

        foreach (Transform t in allTransforms)
        {
            if (t.CompareTag("ProjectileSpawner"))
            {
                projectileSpawner = t.gameObject;
            }

            if (t.CompareTag("Target"))
            {
                target = t.gameObject;
            }

            if (t.CompareTag("EnemyInside"))
            {
                anim = t.GetComponent<Animator>();
            }

            if (t.CompareTag("Weapon"))
            {
                weapon = t.gameObject;
            }

            if (t.CompareTag("Materials"))
            {
                objRenderer = t.GetComponent<Renderer>();;
            }
        }

        player = GameObject.Find("PlayerController");
        cam = Camera.main;

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

    private void OnEnable()
    {
        if (!isShooting && shootingCoroutine != null)
        {
            shootingCoroutine = StartCoroutine(ShootBullets());
        }
        
        if (!isShot && damageRoutine != null)
        {
            damageRoutine = StartCoroutine(Damage());
        }
    }

    private void OnDisable()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            isShooting = false;
        }

        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            isShot = false;
        }
    }

    private void Update()
    {
        visible = IsVisible(cam, gameObject);

        if(visible && allManager.Play)
        {
            mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
                
            foreach (Collider enemyCollider in enemyColliders)
            {
                bounds = enemyCollider.bounds;
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

            if (player.GetComponent<Collider>().bounds.Intersects(weapon.GetComponent<Collider>().bounds))
            {
                anim.SetTrigger("stab");
            }

            if (!isWalking)
            {
                anim.SetBool("walk", false);
                direction = player.transform.position - transform.position;
                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    Quaternion enemyRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Euler(0, enemyRotation.eulerAngles.y, 0);
                }

                if (!isLocked)
                {
                    target.transform.position = player.transform.position + new Vector3(0,1f,0);
                }

                if (!isShooting)
                {
                    shootingCoroutine = StartCoroutine(ShootBullets());
                }
            }
            else
            {
                anim.SetBool("walk", true);
                transform.Translate(new Vector3(0f,0f,1f) * 2f * Time.deltaTime, Space.Self);
            }

            if (life <= 0)
            {
                mouseFollower.IsAim = false;
                ++allManager.KillCounter;


                if (allManager.KillCounter % 7 == 0)
                {
                    GameObject pooledProjectile = SeizedPooler.SharedInstance.GetPooledObject();
                    if (pooledProjectile != null)
                    {
                        pooledProjectile.SetActive(true);
                        pooledProjectile.transform.position = player.transform.position + new Vector3(0,0,2);
                    } 
                }
                else if (allManager.KillCounter % 5 == 0)
                {
                    GameObject pooledProjectile = HeartPooler.SharedInstance.GetPooledObject();
                    if (pooledProjectile != null)
                    {
                        pooledProjectile.SetActive(true);
                        pooledProjectile.transform.position = player.transform.position + new Vector3(0,0,2);
                    } 
                }


                gameObject.SetActive(false);

                allManager.IsWin = true;
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

    IEnumerator ShootBullets()
    {
        isShooting = true;
        
        isLocked = true;

        yield return new WaitForSeconds(shootCooldown - difficulty / 5f);

        for (int i = 0; i < bulletsToShoot; i++)
        {
            GameObject pooledProjectile = BulletPoolerEnemyT.SharedInstance.GetPooledObject();
            if (pooledProjectile != null)
            {
                pooledProjectile.SetActive(true);
                pooledProjectile.transform.position = projectileSpawner.transform.position;
                pooledProjectile.GetComponent<Rigidbody>().velocity = projectileSpawner.transform.forward * bulletVelocity;
            }
            yield return new WaitForSeconds(shootInterval);
        }

        isLocked = false;

        isShooting = false;
    }

    IEnumerator Damage()
    {
        SetColors(newColor);
        yield return new WaitForSeconds(0.25f);
        ResetColors();
        
        isShot = false;
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Stop"))
        {
            isWalking = false;
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BulletPlayer"))
        {
            if(!isShot)
            {
                life--;
                isShot = true;
                damageRoutine = StartCoroutine(Damage());
            }
        }
    }
}