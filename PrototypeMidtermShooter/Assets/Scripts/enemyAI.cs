using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, iDamageable
{

    [Header("Components")]
    [Header("----------------------------------------------")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;

    [Header("Enemy Atrobutes")]
    [Header("----------------------------------------------")]
    [SerializeField] int HP;
    [SerializeField] int viewAngle;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int roamRadius;



    [Header("Enemy Atrobutes")]
    [Header("----------------------------------------------")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;


    bool canShoot;
    bool playerInRange;
    Vector3 playerDir;
    Vector3 startingPos;
    float StoppingDisOrig;


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        StoppingDisOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = gameManager.instance.player.transform.position - transform.position;

        //i fin range move towards him
        if (playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);

            canSeePlayer();
            facePlayer();
        }

        //dont move if not in range
        else if (agent.remainingDistance < 0.1f)
        {
            StartCoroutine(roam());
        }



    }

    IEnumerator roam()
    {
        agent.stoppingDistance = 0;
        Vector3 randomDir = Random.insideUnitSphere * roamRadius;

        randomDir += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDir, out hit, roamRadius, 1);
        NavMeshPath path = new NavMeshPath();


        agent.CalculatePath(hit.position, path);
        agent.SetPath(path);

        yield return new WaitForSeconds(3);
    }

    void facePlayer()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            //doenst work for our game just is funny
            // transform.LookAt(gameManager.instance.player.transform.position);

            playerDir.y = 0;
            var rotation = Quaternion.LookRotation(playerDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
        }
    }

    void canSeePlayer()
    {
        float angle = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            Debug.DrawRay(transform.position, playerDir);
            if (hit.collider.CompareTag("Player") && canShoot && angle <= viewAngle)
            {
                StartCoroutine(shoot());
            }
        }
    }

    //enter the enemy range
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            canShoot = true;
            agent.stoppingDistance = StoppingDisOrig;
        }
    }

    //leave the enemy range
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;

        playerInRange = true;

        StartCoroutine(flashColor());
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator flashColor()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
    }

    IEnumerator shoot()
    {
        canShoot = false;

        Instantiate(bullet, transform.position, bullet.transform.rotation);

        yield return new WaitForSeconds(shootRate);

        canShoot = true;
    }
}
