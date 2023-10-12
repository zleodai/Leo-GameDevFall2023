using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingMovement : MonoBehaviour
{
    public static SwingMovement instance = null;

    [Header("Refrences")]
    private LineRenderer lr;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    public PlayerMovement playerMovement;

    [Header("Spring")]
    public float spring;
    public float damper;
    public float massScale;
    public float minDistance;
    public float maxDistance;

    //[Header("Grapple")]
    //public float grappleSpeed;
    //public float grappleStrength;

    [Header("Swinging")]
    public float maxSwingDistance;
    private Vector3 swingPoint;
    private SpringJoint joint;
    private Vector3 currentGrapplePosition;

    [Header("OdmGear")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;

    public bool odmRight, odmLeft, odmForward, odmBackward, odmShorten;
    public bool swinging;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        playerMovement = FindFirstObjectByType<PlayerMovement>();
        lr = GetComponent<LineRenderer>();

        odmRight = false;
        odmLeft = false;
        odmForward = false;
        odmBackward = false;
        odmShorten = false;
    }

    //private void Update()
    //{
    //    if (playerMovement.swinging)
    //    {
    //        Vector3 moveVec = (swingPoint - player.transform.position).normalized;
    //        moveVec += cam.transform.forward * grappleSpeed;
    //        player.GetComponent<Rigidbody>().AddForce(moveVec * grappleStrength * Time.deltaTime, ForceMode.VelocityChange);

    //        if (Vector3.Distance(transform.position, swingPoint) > maxSwingDistance)
    //        {
    //            StopSwing();
    //        }
    //    }
    //}

    private void Update()
    {
        OdmGearMovement();
        swinging = playerMovement.swinging;
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    public void StartSwing()
    {
        playerMovement.swinging = true;
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;
            joint.anchor = new Vector3(0, 0, 0);
            joint.tag = "qGrapple";

            float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

            joint.maxDistance = distanceFromPoint * maxDistance;
            joint.minDistance = distanceFromPoint * minDistance;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }

    public void StopSwing()
    {
        if (playerMovement.swinging)
        {
            playerMovement.swinging = false;
            playerMovement.inAir = true;
        }
        SpringJoint[] joints = player.GetComponentsInChildren<SpringJoint>();
        foreach (Joint joint in joints)
        {
            if(joint.tag == "qGrapple") Destroy(joint);
        }
        lr.positionCount = 0;

        odmRight = false;
        odmLeft = false;
        odmForward = false;
        odmBackward = false;
        odmShorten = false;
    }

    public void killRope()
    {
        SpringJoint[] joints = player.GetComponentsInChildren<SpringJoint>();
        foreach (Joint joint in joints)
        {
            Destroy(joint);
        }
        lr.positionCount = 0;
    }

    private void DrawRope()
    {
        if (!playerMovement.swinging) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public void OdmGearMovement()
    {
        if (odmRight) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);

        if (odmLeft) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        if (odmForward) rb.AddForce(orientation.forward * forwardThrustForce * Time.deltaTime);

        if (odmBackward) rb.AddForce(-orientation.forward * forwardThrustForce * Time.deltaTime);

        if (odmShorten)
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * maxDistance;
            joint.minDistance = distanceFromPoint * minDistance;
        }
    }
}
