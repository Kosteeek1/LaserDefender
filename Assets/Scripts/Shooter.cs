using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 5f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private bool useAI;

    public bool isFiring;

    private Coroutine firingCoroutine;

    void Start()
    {
        if (useAI)
        {
            isFiring = true;
        }
    }


    void Update()
    {
        Fire();
    }

    void Fire()
    {
        if (isFiring && firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinously());
        }
        else if (!isFiring && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    IEnumerator FireContinously()
    {
        while (true)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, quaternion.identity);
            Rigidbody2D projectileRigidbody = projectileInstance.GetComponent<Rigidbody2D>();
            if (projectileRigidbody != null)
            {
                if (useAI)
                {
                    projectileRigidbody.velocity = -transform.up * projectileSpeed;
                }
                else if (!useAI)
                {
                    projectileRigidbody.velocity = transform.up * projectileSpeed;
                }
            }

            Destroy(projectileInstance, projectileLifetime);
            yield return new WaitForSeconds(fireRate);
        }
    }
}