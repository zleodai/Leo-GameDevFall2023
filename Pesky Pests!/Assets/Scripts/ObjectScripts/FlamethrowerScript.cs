using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerScript : MonoBehaviour, ItemInterface
{
    public bool firing;

    private LayerMask enemyLayer;
    private LayerMask groundLayer;

    private MeshRenderer[] meshRenderers;
    private BoxCollider flamethrowerCollider;
    private Rigidbody flamethrowerRigidbody;
    private GameObject playerCamera;
    private ParticleSystem[] flamethrowerEffects;

    [Header("Flamethrower Stats")]
    public float range;
    public float damagePerSecond;

    private void Awake()
    {
        enemyLayer = LayerMask.GetMask("EnemyLayer");
        groundLayer = LayerMask.GetMask("GroundLayer");

        flamethrowerCollider = GetComponentInChildren<BoxCollider>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        flamethrowerRigidbody = GetComponent<Rigidbody>();

        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");

        flamethrowerEffects = GetComponentsInChildren<ParticleSystem>();

        firing = false;

        range = 20f;
        damagePerSecond = 20f;
    }

    private void Update()
    {
        updateState();
    }

    public void interact()
    {
        if (!firing)
        {
            firing = true;
        }
        else
        {
            firing = false;
        }
    }

    public void updateState()
    {
        if (firing)
        {
            enableEffects(true);


            RaycastHit enemyHit;
            bool hitEnemy = false;
            if (Physics.SphereCast(transform.position + playerCamera.transform.forward.normalized * -2, 2f, playerCamera.transform.forward, out enemyHit, range +2, enemyLayer))
            {
                hitEnemy = true;
            }

            RaycastHit groundHit;
            Physics.Raycast(transform.position, playerCamera.transform.forward.normalized, out groundHit, range, groundLayer);

            Vector3 defaultPoint = new Vector3(0f, 0f, 0f);
            float hitGroundDistance = Vector3.Distance(transform.position, groundHit.point);
            float hitEnemyDistance = Vector3.Distance(transform.position, enemyHit.point);
            if (groundHit.point == defaultPoint) hitGroundDistance = 9999;
            if (enemyHit.point == defaultPoint) hitEnemyDistance = 9999;

            Debug.DrawRay(transform.position + playerCamera.transform.forward.normalized * -2, playerCamera.transform.forward.normalized * (range +2), Color.red, 0.1f);

            if (hitEnemy && hitGroundDistance > hitEnemyDistance)
            {
                PestInterface enemyScript = enemyHit.collider.gameObject.transform.parent.gameObject.GetComponent<PestInterface>();
                if (enemyScript != null)
                {
                    enemyScript.AddDebuff(PestInterface.Debuff.OnFire);
                    enemyScript.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }
        else
        {
            enableEffects(false);
        }
    }

    public void equip()
    {
        firing = false;
        enableRenderers(true);

        gameObject.transform.parent = playerCamera.transform;

        gameObject.transform.localPosition = new Vector3(0.448f, -0.503f, 0.665f);
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(0, 0, 0);
        gameObject.transform.localRotation = rotation;
    }

    public void unequip()
    {
        firing = false;
        enableRenderers(false);

        gameObject.transform.parent = null;
    }

    public void pickup()
    {
        firing = false;
        enableRenderers(false);
        flamethrowerRigidbody.isKinematic = true;
        flamethrowerCollider.enabled = false;
    }

    public void drop()
    {
        firing = false;
        enableRenderers(true);
        flamethrowerRigidbody.isKinematic = false;
        flamethrowerCollider.enabled = true;

        gameObject.transform.parent = null;
    }

    private void enableRenderers(bool enable)
    {
        if (enable)
        {
            foreach(MeshRenderer renderer in meshRenderers)
            {
                renderer.enabled = true;
            }
        }
        else
        {
            foreach (MeshRenderer renderer in meshRenderers)
            {
                renderer.enabled = false;
            }
        }
    }

    private void enableEffects(bool enable)
    {
        if (enable)
        {
            foreach(ParticleSystem particle in flamethrowerEffects)
            {
                particle.enableEmission = true;
            }
        } else
        {
            foreach (ParticleSystem particle in flamethrowerEffects)
            {
                particle.enableEmission = false;
            }
        }
    }
}
