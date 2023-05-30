using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using Mono.Cecil;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Shooter : MonoBehaviour
{
    [Header("General")] [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 5f;
    [SerializeField] private float baseFireRate = 0.2f;

    [Header("AI")] [SerializeField] private float fireRateVariant = 0f;
    [SerializeField] private float minimumFireRate = 0.1f;
    [SerializeField] private bool useAI;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    [HideInInspector] public bool isFiring;

    private Coroutine firingCoroutine;

    void Awake()
    {
        InitBounds();
    }

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
                if (useAI && !IsOnScreen(projectileInstance))
                {
                    Destroy(projectileInstance);
                }
                else if (useAI && IsOnScreen(projectileInstance))
                {
                    projectileRigidbody.velocity = -transform.up * projectileSpeed;
                }
                else if (!useAI)
                {
                    projectileRigidbody.velocity = transform.up * projectileSpeed;
                }
            }

            Destroy(projectileInstance, projectileLifetime);
            float fireDelay = Random.Range(baseFireRate - fireRateVariant, fireRateVariant + baseFireRate);
            float timeToNextProjectile = Mathf.Clamp(fireDelay, minimumFireRate, float.MaxValue);

            yield return new WaitForSeconds(timeToNextProjectile);
        }
    }

    void InitBounds()
    {
        Camera mainCamera = Camera.main;

        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
    }

    bool IsOnScreen(GameObject projectile)
    {
        Rigidbody2D body = projectile.GetComponent<Rigidbody2D>();
        if (body.position.x < maxBounds.x && body.position.x > minBounds.x && body.position.y < maxBounds.y &&
            body.position.y > minBounds.y)
        {
            return true;
        }

        return false;
    }
}