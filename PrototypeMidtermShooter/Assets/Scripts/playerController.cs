using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamagable
{

    [Header("Components")]
    [SerializeField] CharacterController controller;



    [Header("Player Attributes")]
    [Header("---------------------------")]
    [Range(5, 20)] [SerializeField] int HP;
    [Range(3, 6)] [SerializeField] float playerSpeed;
    [Range(1.5f, 4)] [SerializeField] float sprintMult;
    [Range(6, 10)] [SerializeField] float jumpHeight;
    [Range(1, 4)] [SerializeField] int jumps;
    [Range(15, 30)] [SerializeField] float gravityValue;

    [Header("Player Weapon")]
    [Header("---------------------------")]
    [Range(0.1f, 3)] [SerializeField] float shootrate;
    [Range(1, 10)] [SerializeField] int weaponDamage;

    [Header("Effects")]
    [Header("---------------------------")]
    [SerializeField] GameObject hitEffectSpark;
    [SerializeField] GameObject muzzleFlash;
    


    //[SerializeField] GameObject cube;


    bool isSprint = false;
    float playerSpeedOrig;
    int timesJumped;
    private Vector3 playerVelocity;
    Vector3 move;
    bool canshoot = true;
    int HPOrig;
    Vector3 playerSpawnPos;


    private void Start()
    {

        playerSpeedOrig = playerSpeed;
        HPOrig = HP;
        playerSpawnPos = transform.position;
    }

    void Update()
    {
        if (!gamemanager.instance.paused)
        {
            MovePlayer();
            sprint();
            StartCoroutine(shoot());
        }
    }
    private void MovePlayer()
    {

        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            playerVelocity.y -= 2;
        }

        //if we land reset the player velocity and the jump counter
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            timesJumped = 0;
            playerVelocity.y = 0f;
        }


        //get the input from Unity's input system
        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));


        //if (move != Vector3.zero)
        //{
        //    gameObject.transform.forward = move;
        //}

        // Changes the height position of the player..
        controller.Move(move * Time.deltaTime * playerSpeed);


        if (Input.GetButtonDown("Jump") && timesJumped < jumps)
        {
            timesJumped++;
            playerVelocity.y = jumpHeight;
        }

        //gravity
        playerVelocity.y -= gravityValue * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprint = true;
            playerSpeed = playerSpeed * sprintMult;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprint = false;
            playerSpeed = playerSpeedOrig;
        }
    }

    IEnumerator shoot()
    {
        RaycastHit hit;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100, Color.red);
        if (Input.GetButtonDown("Shoot") && canshoot)
        {


            canshoot = false;


            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit))
            {
                Instantiate(hitEffectSpark, hit.point, hitEffectSpark.transform.rotation);
                //if (transform.CompareTag("Finish"))
                //{
                //    Destroy(hit.transform.gameObject);
                //}
                //else
                //{
                //    Instantiate(cube, hit.point, transform.rotation);
                //}

                IDamagable isDamageable = hit.collider.GetComponent<IDamagable>();
                if (hit.collider.GetComponent<IDamagable>() != null)
                {
                    if (hit.collider is SphereCollider)
                    {
                        isDamageable.takeDamage(100000);
                    }
                    else
                        isDamageable.takeDamage(weaponDamage);
                }
            }
            muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 350));
            muzzleFlash.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            muzzleFlash.SetActive(false);

            yield return new WaitForSeconds(shootrate);
            canshoot = true;
        }
    }
    public void takeDamage(int dmg)
    {
        HP -= dmg;

        StartCoroutine(damageFlash());
        gamemanager.instance.HPBar.fillAmount = (float)HP / (float)HPOrig;

        if (HP <= 0)
        {
            gamemanager.instance.playerDead();
        }
    }

    IEnumerator damageFlash()
    {
        gamemanager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageFlash.SetActive(false);

    }


    public void giveHP(int amount)
    {
        if (HP < HPOrig)
        {
            HP += amount;
        }

        if (HP > HPOrig)
        {
            HP = HPOrig;
        }
        updatePlayerHp();
    }

    public void updatePlayerHp()
    {
        gamemanager.instance.HPBar.fillAmount = (float)HP / (float)HPOrig;
    }

    public void respawn()
    {
        HP = HPOrig;
        updatePlayerHp();
        controller.enabled = false;
        transform.position = playerSpawnPos;
        controller.enabled = true;
    }




}
