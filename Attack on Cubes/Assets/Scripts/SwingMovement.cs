using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingMovement : MonoBehaviour
{
    public static SwingMovement instance = null;

    [Header("Refrences")]
    private LineRenderer lr;
    public Transform gunTip;
    public Transform cam, player;
    public LayerMask whatIsGrappleable;
    public PlayerMovement playerMovement;

    [Header("Spring")]
    public float spring;
    public float damper;
    public float massScale;
    public float minDistance;
    public float maxDistance;

    [Header("Swinging")]
    public float maxSwingDistance;
    private Vector3 swingPoint;
    private SpringJoint joint;
    private Vector3 currentGrapplePosition;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;

    [Header("OdmGear")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;
    public float heldMult;
    public float burstMult;

    public bool odmRight, odmLeft, odmForward, odmBackward, odmShorten;
    private bool swinging;

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

    private void Update()
    {
        OdmGearMovement();
        CheckForSwingPoints();

        swinging = playerMovement.swinging;
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    public void StartSwing()
    {
        if (predictionHit.point == Vector3.zero) return;

        playerMovement.swinging = true;

        swingPoint = predictionHit.point;
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
        if (!swinging || joint == null) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    private void CheckForSwingPoints()
    {
        if (joint != null || swinging) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxSwingDistance, whatIsGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, whatIsGrappleable);

        Vector3 realHitPoint;

        if (raycastHit.point != Vector3.zero) realHitPoint = raycastHit.point;

        else if (sphereCastHit.point != Vector3.zero) realHitPoint = sphereCastHit.point;

        else realHitPoint = Vector3.zero;

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        } else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    public void OdmGearMovement()
    {
        if (!swinging || joint == null) return;

        if (odmRight) rb.AddForce(orientation.right * horizontalThrustForce * heldMult * Time.deltaTime);

        if (odmLeft) rb.AddForce(-orientation.right * horizontalThrustForce * heldMult * Time.deltaTime);

        if (odmForward) rb.AddForce(orientation.forward * forwardThrustForce * heldMult * Time.deltaTime);

        if (odmBackward) rb.AddForce(-orientation.forward * forwardThrustForce * heldMult * Time.deltaTime);

        if (odmShorten)
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * maxDistance;
            joint.minDistance = distanceFromPoint * minDistance;
        }
    }

    public void OdmRightBurst()
    {
        if (!swinging || joint == null) return;
        rb.velocity = orientation.right * horizontalThrustForce * burstMult;
    }

    public void OdmLeftBurst()
    {
        if (!swinging || joint == null) return;
        rb.velocity = -orientation.right * horizontalThrustForce * burstMult;
    }

    public void OdmForwardtBurst()
    {
        if (!swinging || joint == null) return;
        rb.velocity = orientation.forward * horizontalThrustForce * burstMult;
    }

    public void OdmBackwardBurst()
    {
        if (!swinging || joint == null) return;
        rb.velocity = -orientation.forward * horizontalThrustForce * burstMult;
    }
}
