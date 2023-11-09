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

    private void Awake()
    {
        lightMeshRenderer = transform.Find("Light").gameObject.GetComponent<MeshRenderer>();
        lightRenderer = lightMeshRenderer.gameObject.transform.Find("Light").gameObject.GetComponent<Light>();
        flashlightCollider = GetComponent<BoxCollider>();
        flashlightRigidBody = GetComponent<Rigidbody>();
        state = false;
        lightMeshRenderer.material = unlitLight;
        lightRenderer.enabled = false;
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
        flashlightCollider.enabled = false;
        flashlightRigidBody.isKinematic = true;
    }

    public void unequip()
    {
        flashlightCollider.enabled = true;
        flashlightRigidBody.isKinematic = false;
    }
}
