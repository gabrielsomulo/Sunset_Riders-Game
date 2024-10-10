using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI lifeTXt;
    [SerializeField] private TextMeshProUGUI giftTXt;

    private bool isPressed = false, isOnGround = false, isSeized = false;
    private float time, verticalIput, horizontalIput, shootCooldown = 1;
    private int gift, bulletVelocity = 30;

    private readonly float jumpForce = 900, speed = 5;
    private readonly Color newColor = Color.red;

    private Color[] originalColors;
    private Vector3 checkPoint = new Vector3(-7.5f, 2, 100);
    private Rigidbody playerRb;
    private GameObject projectileSpawner, weapon;
    private Transform[] allTransforms;
    private AllManager allManager;
    private Animator anim; 
    private Material[] materials;
    private Renderer objRenderer;
    private MouseFollower mouseFollower;

    private int life = 8;
    public int Life 
    {
        get { return life; }
        set { life = value; }
    }

    private void Start()
    {
         allManager = FindObjectOfType<AllManager>();

         playerRb = GetComponent<Rigidbody>();

         life -= allManager.Difficulty*2;
         lifeTXt.text = ""+life;
         giftTXt.text = gift+".R$";

        allTransforms = GetComponentsInChildren<Transform>();
        mouseFollower = GetComponentInChildren<MouseFollower>();

        foreach (Transform t in allTransforms)
        {
            if (t.CompareTag("ProjectileSpawner"))
            {
                projectileSpawner = t.gameObject;
            }

            if (t.CompareTag("PlayerInside"))
            {
                anim = t.GetComponent<Animator>();
            }

            if (t.CompareTag("Weapon"))
            {
                weapon = t.gameObject;
            }
        }

        objRenderer = weapon.GetComponent<Renderer>();
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

    private void FixedUpdate()
    {
        if (allManager.Play)
        {
            time += Time.deltaTime;

            horizontalIput = Input.GetAxis("Horizontal");
            verticalIput = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(verticalIput, 0, -horizontalIput);
            direction.Normalize();

            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (direction != Vector3.zero)
            {
                transform.forward = -direction;
            }

            if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
            {
             playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
             isOnGround = false;
            }

            if (life <= 0)
            {
                allManager.IsGameOver = true;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)  && isPressed==false)
            {
                isPressed = true;
                anim.SetBool("isPressed",true);
            }


            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) && isPressed) 
            {
                isPressed = false;
                 anim.SetBool("isPressed",false);
            }

            if (!isSeized && time >= shootCooldown && allManager.Play && Input.GetMouseButtonDown(0))
            {
                SpawnBullet(); 
            }
            else if (isSeized && time >= shootCooldown && allManager.Play && Input.GetMouseButton(0))
            {
                SpawnBullet();
            }
        }
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

    private void SpawnBullet()
    {
        GameObject pooledProjectile = BulletPoolerPlayer.SharedInstance.GetPooledObject();
        if(pooledProjectile != null)
        {
            pooledProjectile.SetActive(true);
            pooledProjectile.transform.position = projectileSpawner.transform.position;
            pooledProjectile.GetComponent<Rigidbody>().velocity = projectileSpawner.transform.forward * bulletVelocity;       
        }
        time = 0;
    }

        private void OnCollisionEnter(Collision other) 
    {
       
        if (other.gameObject.CompareTag("Ground")) 
        {
           isOnGround = true;
        }
            
        if (other.gameObject.CompareTag("CheckPoint") || other.gameObject.CompareTag("Wave"))
        {
            checkPoint = other.transform.position;
            other.gameObject.SetActive(false);
                
        }

        if (other.gameObject.CompareTag("Life"))
        {
            other.gameObject.SetActive(false);
            life++;
            lifeTXt.text = ""+life;
        }

        if (other.gameObject.CompareTag("Power"))
        {
            other.gameObject.SetActive(false);
            gift += 100;
            giftTXt.text = gift+".R$";
            StartCoroutine(SeizedCooldown());
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("BulletEnemy") || other.gameObject.CompareTag("Death") || other.gameObject.CompareTag("AreaDamage"))
        {
            mouseFollower.IsAim = false;
            life--;
            lifeTXt.text = ""+life;
            transform.position = checkPoint;
        }
    }

    IEnumerator SeizedCooldown()
    {
        SetColors(newColor);
        shootCooldown = 0.25f;
        isSeized = true;
        yield return new WaitForSeconds(7);
        isSeized = false;
        ResetColors();
        shootCooldown = 1.25f;
    }
}
