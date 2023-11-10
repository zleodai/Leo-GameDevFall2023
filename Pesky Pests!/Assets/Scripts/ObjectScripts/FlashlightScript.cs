using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightScript : MonoBehaviour, ItemInterface
{
    public Material unlitLight;
    public Material litLight;

    public bool state;

    private MeshRenderer lightMeshRenderer;
    private Light lightRenderer;
    private BoxCollider flashlightCollider;
    private Rigidbody flashlightRigidBody;
    private MeshRenderer flashlightMeshRenderer;
    private GameObject playerCamera;

    private void Awake()
    {
        lightMeshRenderer = transform.Find("Light").gameObject.GetComponent<MeshRenderer>();
        lightRenderer = lightMeshRenderer.gameObject.transform.Find("Light").gameObject.GetComponent<Light>();
        flashlightCollider = GetComponent<BoxCollider>();
        flashlightRigidBody = GetComponent<Rigidbody>();
        flashlightMeshRenderer = GetComponent<MeshRenderer>();
        if (state)
        {
            lightMeshRenderer.material = litLight;
            lightRenderer.enabled = true;
        }
        else
        {
            lightMeshRenderer.material = unlitLight;
            lightRenderer.enabled = false;
        }

        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    public void interact()
    {
        if (!state)
        {
            lightMeshRenderer.material = litLight;
            lightRenderer.enabled = true;
            state = true;
        }
        else
        {
            lightMeshRenderer.material = unlitLight;
            lightRenderer.enabled = false;
            state = false;
        }
    }

    public void equip()
    {
        flashlightMeshRenderer.enabled = true;
        lightMeshRenderer.enabled = true;
        gameObject.transform.parent = playerCamera.transform;
        gameObject.transform.localPosition = new Vector3(0.448f, -0.503f, 0.665f);
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(0, 90, 90);
        gameObject.transform.localRotation = rotation;
        state = false;
        lightMeshRenderer.material = unlitLight;
        lightRenderer.enabled = false;
    }

    public void unequip()
    {
        flashlightMeshRenderer.enabled = false;
        lightMeshRenderer.enabled = false;
        state = false;
        lightMeshRenderer.material = unlitLight;
        lightRenderer.enabled = false;
        gameObject.transform.parent = null;
    }

    public void pickup()
    {
        flashlightCollider.enabled = false;
        flashlightRigidBody.isKinematic = true;
        flashlightMeshRenderer.enabled = false;
        lightMeshRenderer.enabled = false;
        lightMeshRenderer.material = unlitLight;
        lightRenderer.enabled = false;
    }

    public void drop()
    {
        flashlightCollider.enabled = true;
        flashlightRigidBody.isKinematic = false;
        flashlightMeshRenderer.enabled = true;
        lightMeshRenderer.enabled = true;
        gameObject.transform.parent = null;
    }
}
