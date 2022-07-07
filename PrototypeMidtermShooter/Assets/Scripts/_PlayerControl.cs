using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _PlayerControl : MonoBehaviour, iDamageable
{
    [Header("Components")]
    [SerializeField] CharacterController controller;

    [Header("Player Atrobutes")]
    [Header("----------------------------------------------")]
    [Range(5, 20)] [SerializeField] int HP;
    [Range(3, 6)] [SerializeField] float playerSpeed;
    [Range(1.5f, 4.5f)] [SerializeField] float sprintMult;
    [Range(6, 10)] [SerializeField] float jumpHeight;
    [Range(15, 30)] [SerializeField] float gravityValue;
    [Range(1, 4)] [SerializeField] int jumps;

    [Header("Player Weapon Stats")]
    [Header("----------------------------------------------")]
    [Range(0.1f, 3)] [SerializeField] float shootRate;
    [Range(1, 10)] [SerializeField] int weaponDamage;

    [Header("Effects")]
    [Header("----------------------------------------------")]
    [SerializeField] GameObject hitEffectSpark;
    [SerializeField] GameObject muzzleFlash;


    bool isSprint = false;
    float playerSpeedOg;
    int Times_jump;
    Vector3 playerVelocity;
    Vector3 move;
    bool canShoot = true;
    int hpOriginal;
    Vector3 playerSpawnPos;

    private void Start()
    {
        playerSpeedOg = playerSpeed;
        hpOriginal = HP;
        playerSpawnPos = transform.position;
    }

    void Update()
    {
        if (!gameManager.instance.paused)
        {
            MovePLayer();
            Sprint();
            StartCoroutine(shoot());
        }

    }

    private void MovePLayer()
    {

        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            playerVelocity.y -= 3;
        }


        // groundedPlayer = controller.isGrounded;
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            Times_jump = 0;
            playerVelocity.y = 0f;
        }

        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && Times_jump < jumps)
        {
            Times_jump++;
            playerVelocity.y = jumpHeight;
        }

        //adding gravity
        playerVelocity.y -= gravityValue * Time.deltaTime;


        controller.Move(playerVelocity * Time.deltaTime);
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprint = true;
            playerSpeed = playerSpeed * sprintMult;
        }

        else if (Input.GetButtonUp("Sprint"))
        {
            isSprint = false;
            playerSpeed = playerSpeedOg;
        }
    }

    IEnumerator shoot()
    {
        // Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * 100, Color.red);
        if (Input.GetButton("Shoot") && canShoot)
        {
            canShoot = false;

            RaycastHit hit;



            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit))
            {

                Instantiate(hitEffectSpark, hit.point, hitEffectSpark.transform.rotation);
                if (hit.collider.GetComponent<iDamageable>() != null)
                {
                    iDamageable isDamageable = hit.collider.GetComponent<iDamageable>();
                    if (hit.collider is SphereCollider)
                    {
                        isDamageable.takeDamage(10000);
                    }
                    else
                    {
                        isDamageable.takeDamage(weaponDamage);
                    }


                }
            }

            muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            muzzleFlash.SetActive(true);
            yield return new WaitForSeconds(.05f);
            muzzleFlash.SetActive(false);

            yield return new WaitForSeconds(shootRate);
            canShoot = true;
        }

    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        updatePlayerHP();

        StartCoroutine(damageFlash());

        if (HP <= 0)
        {
            gameManager.instance.playerDead();
        }
    }

    IEnumerator damageFlash()
    {
        gameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageFlash.SetActive(false);

    }

    public void giveHP(int amount)
    {
        if (HP < hpOriginal)
        {
            HP += amount;
            if (HP > hpOriginal)
            {
                HP = hpOriginal;
            }
        }
        updatePlayerHP();
    }

    public void updatePlayerHP()
    {
        gameManager.instance.HPBar.fillAmount = (float)HP / (float)hpOriginal;
    }

    public void respawn()
    {

        HP = hpOriginal;
        updatePlayerHP();
        controller.enabled = false;
        transform.position = playerSpawnPos;
        controller.enabled = true;
    }
}
