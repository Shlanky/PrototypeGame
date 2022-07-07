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



    [Header("Enemy Atrobutes")]
    [Header("----------------------------------------------")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;


    bool canShoot = true;
    bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (agent.remainingDistance <= agent.stoppingDistance & canShoot)
        {
            StartCoroutine(shoot());
        }
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;

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
