using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingMovement : MonoBehaviour
{
    public static SwingMovement instance = null;

    [Header("Refrences")]
    public List<LineRenderer> lrs;
    public Transform[] gunTips;
    public Transform cam, player;
    public LayerMask whatIsGrappleable;
    public PlayerMovement playerMovement;
    public ParticleSystem odmSmoke;
    public ParticleSystem burstSmoke;
    private float burstSmokeDuration;
    public Transform orientation;
    public bool odmSmokeOn;
    public bool burstSmokeOn;

    [Header("Spring")]
    public float spring;
    public float damper;
    public float massScale;
    public float minDistance;
    public float maxDistance;

    [Header("Swinging")]
    public float maxSwingDistance;
    private List<Vector3> swingPoints;
    private List<SpringJoint> joints;
    private List<Vector3> currentGrapplePositions;

    [Header("Prediction")]
    public List<RaycastHit> predictionHits;
    public float predictionSphereCastRadius;
    public List<Transform> predictionPoints;

    [Header("DualSwinging")]
    public int amountOfSwingPoints = 2;
    public List<Transform> pointAimers;
    public List<bool> swingsActive;

    [Header("OdmGear")]
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;
    public float heldMult;
    public float burstMult;
    public float noHookMult;

    public bool odmRight, odmLeft, odmForward, odmBackward, odmShorten;
    private bool swinging;

    [Header("Smoke")]
    private bool dashing;
    private float dashingTimeLeft;

    private bool smoking;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        playerMovement = FindFirstObjectByType<PlayerMovement>();

        odmRight = false;
        odmLeft = false;
        odmForward = false;
        odmBackward = false;
        odmShorten = false;

        burstSmoke.Stop();
        odmSmoke.Stop();
    }

    private void Start()
    {
        ListSetup();
    }

    private void Update()
    {
        OdmGearMovement();
        CheckForSwingPoints();

        swinging = playerMovement.swinging;

        if(dashing && dashingTimeLeft > 0f)
        {
            Instantiate(burstSmoke, transform.position, transform.rotation);
        }

        if(dashingTimeLeft <= 0f)
        {
            dashing = false;
        }

        dashingTimeLeft -= Time.deltaTime;

        if (smoking)
        {
            Instantiate(odmSmoke, transform.position, transform.rotation);
        }

        if (!(odmRight || odmLeft || odmForward || odmBackward || odmShorten))
        {
            smoking = false;
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void ListSetup()
    {
        predictionHits = new List<RaycastHit>();

        swingPoints = new List<Vector3>();
        joints = new List<SpringJoint>();

        swingsActive = new List<bool>();

        currentGrapplePositions = new List<Vector3>();

        for (int i = 0; i < amountOfSwingPoints; i++)
        {
            predictionHits.Add(new RaycastHit());
            joints.Add(null);
            swingPoints.Add(Vector3.zero);
            swingsActive.Add(false);
            currentGrapplePositions.Add(Vector3.zero);
        }
    }

    public void StartSwing(int grappleIndex)
    {
        if (predictionHits[grappleIndex].point == Vector3.zero) return;

        playerMovement.swinging = true;
        swingsActive[grappleIndex] = true;

        swingPoints[grappleIndex] = predictionHits[grappleIndex].point;
        joints[grappleIndex] = player.gameObject.AddComponent<SpringJoint>();
        joints[grappleIndex].autoConfigureConnectedAnchor = false;
        joints[grappleIndex].connectedAnchor = swingPoints[grappleIndex];
        joints[grappleIndex].anchor = new Vector3(0, 0, 0);

        float distanceFromPoint = Vector3.Distance(player.position, swingPoints[grappleIndex]);

        joints[grappleIndex].maxDistance = distanceFromPoint * maxDistance;
        joints[grappleIndex].minDistance = distanceFromPoint * minDistance;

        joints[grappleIndex].spring = spring;
        joints[grappleIndex].damper = damper;
        joints[grappleIndex].massScale = massScale;

        lrs[grappleIndex].positionCount = 2;
        currentGrapplePositions[grappleIndex] = gunTips[grappleIndex].position;
    }

    public void StopSwing(int grappleIndex)
    {
        if (playerMovement.swinging)
        {
            playerMovement.swinging = false;
            playerMovement.inAir = true;
        }
        swingsActive[grappleIndex] = false;
        lrs[grappleIndex].positionCount = 0;
        Destroy(joints[grappleIndex]);
    }

    private void DrawRope()
    {
        for (int grappleIndex = 0; grappleIndex < amountOfSwingPoints; grappleIndex++)
        {
            if (!swingsActive[grappleIndex])
            {
                lrs[grappleIndex].positionCount = 0;
            } else
            {
                currentGrapplePositions[grappleIndex] = Vector3.Lerp(currentGrapplePositions[grappleIndex], swingPoints[grappleIndex], Time.deltaTime * 8f);

                lrs[grappleIndex].SetPosition(0, gunTips[grappleIndex].position);
                lrs[grappleIndex].SetPosition(1, currentGrapplePositions[grappleIndex]);
            }
        }
    }

    private void CheckForSwingPoints()
    {
        for (int grappleIndex = 0; grappleIndex < amountOfSwingPoints; grappleIndex++)
        {
            if (swingsActive[grappleIndex]) { }
            else
            {
                RaycastHit sphereCastHit;
                Physics.SphereCast(pointAimers[grappleIndex].position, predictionSphereCastRadius, pointAimers[grappleIndex].forward, out sphereCastHit, maxSwingDistance, whatIsGrappleable);

                RaycastHit raycastHit;
                Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, whatIsGrappleable);

                Vector3 realHitPoint;

                if (raycastHit.point != Vector3.zero) realHitPoint = raycastHit.point;

                else if (sphereCastHit.point != Vector3.zero) realHitPoint = sphereCastHit.point;

                else realHitPoint = Vector3.zero;

                if (realHitPoint != Vector3.zero)
                {
                    predictionPoints[grappleIndex].gameObject.SetActive(true);
                    predictionPoints[grappleIndex].position = realHitPoint;
                }
                else
                {
                    predictionPoints[grappleIndex].gameObject.SetActive(false);
                }

                predictionHits[grappleIndex] = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
            }
        }
    }

    public void OdmGearMovement()
    {
        float tempHorizontalThrustForce = horizontalThrustForce;
        float tempForwardThrustForce = forwardThrustForce;

        if (!swinging || (joints[0] == null && joints[1] == null))
        {
            horizontalThrustForce *= noHookMult;
            forwardThrustForce *= noHookMult;
        }

        if (odmRight)
        {
            rb.AddForce(cam.right * horizontalThrustForce * heldMult * Time.deltaTime);
            smoking = true;
        }

        if (odmLeft)
        {
            rb.AddForce(-cam.right * horizontalThrustForce * heldMult * Time.deltaTime);
            smoking = true;
        }

        if (odmForward)
        {
            rb.AddForce(cam.forward * forwardThrustForce * heldMult * Time.deltaTime);
            smoking = true;
        }

        if (odmBackward)
        {
            rb.AddForce(-cam.forward * forwardThrustForce * heldMult * Time.deltaTime);
            smoking = true;
        }

        horizontalThrustForce = tempHorizontalThrustForce;
        forwardThrustForce = tempForwardThrustForce;

        if (!swinging || (joints[0] == null && joints[1] == null)) return;

        Vector3 pullPoint = GetPullPoint();

        if (odmShorten)
        {
            for (int grappleIndex = 0; grappleIndex < amountOfSwingPoints; grappleIndex++)
            {
                Vector3 directionToPoint = pullPoint - transform.position;
                rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

                float distanceFromPoint = Vector3.Distance(transform.position, swingPoints[grappleIndex]);
                UpdateJoints(distanceFromPoint);
            }
            smoking = true;
        }

        if (odmBackward)
        {
            for (int grappleIndex = 0; grappleIndex < amountOfSwingPoints; grappleIndex++)
            {
                float distanceFromPoint = Vector3.Distance(transform.position, pullPoint) + extendCableSpeed;
                UpdateJoints(distanceFromPoint);
            }
            smoking = true;
        }
    }

    private void UpdateJoints(float distanceFromPoint)
    {
        for (int jointIndex = 0; jointIndex < joints.Count; jointIndex++)
        {
            if (joints[jointIndex] != null)
            {
                joints[jointIndex].maxDistance = distanceFromPoint * maxDistance;
                joints[jointIndex].minDistance = distanceFromPoint * minDistance;
            }
        }
    }

    private Vector3 GetPullPoint()
    {
        Vector3 pullPoint;
        if (swingsActive[0] && !swingsActive[1]) pullPoint = swingPoints[0];
        else if (swingsActive[1] && !swingsActive[0]) pullPoint = swingPoints[1];
        else if (swingsActive[0] && swingsActive[1])
        {
            Vector3 dirToGrapplePoint1 = swingPoints[1] - swingPoints[0];
            pullPoint = swingPoints[0] + dirToGrapplePoint1 * 0.5f;
        }
        else pullPoint = Vector3.zero;
        return pullPoint;
    }

    public void OdmRightBurst()
    {
        if (!swinging || (joints[0] == null && joints[1] == null)) return;
        rb.velocity = cam.right * horizontalThrustForce * burstMult;
        dashing = true;
        dashingTimeLeft = 1f;
    }

    public void OdmLeftBurst()
    {
        if (!swinging || (joints[0] == null && joints[1] == null)) return;
        rb.velocity = -cam.right * horizontalThrustForce * burstMult;
        dashing = true;
        dashingTimeLeft = 1f;
    }

    public void OdmForwardtBurst()
    {
        if (!swinging || (joints[0] == null && joints[1] == null)) return;
        rb.velocity = cam.forward * horizontalThrustForce * burstMult;
        dashing = true;
        dashingTimeLeft = 1f;
    }

    public void OdmBackwardBurst()
    {
        if (!swinging || (joints[0] == null && joints[1] == null)) return;
        rb.velocity = -cam.forward * horizontalThrustForce * burstMult;

        Vector3 pullPoint = GetPullPoint();

        for (int grappleIndex = 0; grappleIndex < amountOfSwingPoints; grappleIndex++)
        {
            float distanceFromPoint = Vector3.Distance(transform.position, pullPoint) + extendCableSpeed * 2f;
            UpdateJoints(distanceFromPoint);
        }

        dashing = true;
        dashingTimeLeft = 1f;
    }
}
