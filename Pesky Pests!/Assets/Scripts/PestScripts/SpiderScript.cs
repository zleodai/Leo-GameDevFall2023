using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RotaryHeart;

public class SpiderScript : MonoBehaviour, PestInterface
{
    [Header("Refrences")]
    private GameManager gameManager;
    private GameObject playerObject;
    private Transform orientation;
    private NavMeshAgent navMeshAgent;

    [Header("Layers")]
    private LayerMask playerLayer;
    private LayerMask itemLayer;
    private LayerMask groundLayer;

    [Header("Behavior")]
    public float lightSightDistance;
    public float sphereForDetectingPlayerRadius;
    public float autoSenseDistance;
    public float sphereCastOffset;
    public float sphereCastOffsett;
    public Vector3[] patrolPoints;
    private bool seenPlayer = false;
    private Vector3 defaultlastSeenLocation = new Vector3(999f, 999f, 990f);
    private Vector3 lastSeenPlayerLocation = new Vector3(999f, 999f, 990f);
    private float timeSinceSeenPlayer = 0;
    public float timeToForgetPlayer = 10;
    private GameObject targetObject = null;
    private Vector3 lastSeenItemLocation = new Vector3(999f, 999f, 999f);
    public float distanceToStalk;
    private float distanceToLastPlayerSpot;
    private int targetPatrolPoint;
    private GameObject[] lightObjects;

    public PestInterface.State state;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        orientation = transform.Find("Orientation").transform;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerLayer = LayerMask.GetMask("PlayerLayer");
        itemLayer = LayerMask.GetMask("Item");
        groundLayer = LayerMask.GetMask("GroundLayer");

        lightSightDistance = 100f;
        sphereForDetectingPlayerRadius = 15f;
        autoSenseDistance = 15f;
        sphereCastOffset = 2f;
        sphereCastOffsett = -11f;
        state = PestInterface.State.Idle;
        gameManager = GameManager.instance;

        distanceToStalk = 50f;

        lightObjects = GameObject.FindGameObjectsWithTag("LightObject");

        GameObject[] patrolPointObjects = GameObject.FindGameObjectsWithTag("SpiderPatrolPoint");
        patrolPoints = new Vector3[patrolPointObjects.Length];
        int index = 0;
        targetPatrolPoint = 0;
        foreach (GameObject obj in patrolPointObjects)
        {
            patrolPoints[index] = obj.transform.position;
            index++;
        }
    }

    private void Update()
    {
        distanceToLastPlayerSpot = Vector3.Distance(playerObject.transform.position, lastSeenPlayerLocation);
        switch (gameManager.gameState)
        {
            case GameManager.GameState.GAMEPLAY:
                TransitionState(PestInterface.State.Idle);
                StateHandeler();
                break;
            default:
                TransitionState(PestInterface.State.Paused);
                break;
        }

        timeSinceSeenPlayer += Time.deltaTime;
        if (timeSinceSeenPlayer > timeToForgetPlayer)
        {
            if (seenPlayer)
            {
                playerForgot();
            }
        }
    }

    public void StateHandeler()
    {
        switch (state)
        {
            case PestInterface.State.Paused:
                break;
            case PestInterface.State.Idle:
                CheckLights();
                if (!seenPlayer)
                {
                    TransitionState(PestInterface.State.Patroling);
                }
                break;
            case PestInterface.State.Patroling:
                CheckLights();
                if (seenPlayer || targetObject == null)
                {
                    if (distanceToLastPlayerSpot > distanceToStalk) TransitionState(PestInterface.State.Stalking);
                    else TransitionState(PestInterface.State.Chasing);
                }
                Vector3 targetPosition = patrolPoints[targetPatrolPoint];
                if (Vector2.Distance(targetPosition, transform.position) < 5f)
                {
                    targetPatrolPoint++;
                    stopMoving();
                    if (targetPatrolPoint >= patrolPoints.Length)
                    {
                        targetPatrolPoint = 0;
                    }
                }
                moveTowards(targetPosition);
                break;
            case PestInterface.State.Stalking:

                break;
            case PestInterface.State.Chasing:

                break;
            case PestInterface.State.Attacking:

                break;
            case PestInterface.State.Running:

                break;
        }
    }
    public void TransitionState(PestInterface.State newState)
    {
        switch (newState)
        {
            case PestInterface.State.Idle:
                if (state == PestInterface.State.Idle)
                {
                    break;
                }
                state = PestInterface.State.Idle;
                //Include code needed to transition state
                break;
            case PestInterface.State.Patroling:
                state = PestInterface.State.Patroling;
                //Include code needed to transition state
                break;
            case PestInterface.State.Stalking:
                state = PestInterface.State.Stalking;
                //Include code needed to transition state
                break;
             case PestInterface.State.Chasing:
                state = PestInterface.State.Chasing;
                //Include code needed to transition state
                break;
            case PestInterface.State.Attacking:

                break;
            case PestInterface.State.Running:

                break;
        }
    }

    private void CheckLights()
    {
        foreach (GameObject light in lightObjects)
        {
            Light lightRender = light.GetComponent<Light>();
            if (lightRender != null)
            {
                if (lightRender.enabled)
                {
                    Vector3 vectorTowardsLight = light.transform.position - transform.position;

                    RaycastHit groundHit;
                    bool hitPlayer = false;
                    RaycastHit playerHit;
                    bool hitItem = false;
                    RaycastHit itemHit;

                    Physics.Raycast(transform.position, vectorTowardsLight.normalized, out groundHit, lightSightDistance, groundLayer);
                    if (Physics.Raycast(transform.position, vectorTowardsLight.normalized, out playerHit, lightSightDistance, playerLayer))
                    {
                        hitPlayer = true;
                    }
                    if (Physics.Raycast(transform.position, vectorTowardsLight.normalized, out itemHit, lightSightDistance, itemLayer))
                    {
                        hitItem = true;
                    }

                    Vector3 defaultPoint = new Vector3(0f, 0f, 0f);
                    float hitGroundDistance = Vector3.Distance(transform.position, groundHit.point);
                    if (groundHit.point == defaultPoint) hitGroundDistance = 9999;

                    float hitPlayerDistance = Vector3.Distance(transform.position, playerHit.point);
                    if (playerHit.point == defaultPoint) hitPlayerDistance = 9999;

                    float hitItemDistance = Vector3.Distance(transform.position, itemHit.point);
                    if (itemHit.point == defaultPoint) hitItemDistance = 9999;

                    if (hitPlayer && !hitItem && hitGroundDistance > hitPlayerDistance)
                    {
                        RaycastHit hit;
                        if (Physics.SphereCast(transform.position + orientation.forward.normalized * sphereCastOffset, sphereForDetectingPlayerRadius, orientation.forward, out hit, lightSightDistance, playerLayer))
                        {
                            playerFound(playerHit.point);
                            Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.red, 0.1f);
                        }
                        else if (Physics.SphereCast(transform.position - orientation.forward.normalized * sphereCastOffsett, sphereForDetectingPlayerRadius, -orientation.forward, out hit, lightSightDistance, playerLayer))
                        {
                            Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.yellow, 0.1f);
                        }
                        else if (hitPlayerDistance <= autoSenseDistance)
                        {
                            playerFound(playerHit.point);
                            Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.red, 0.1f);
                        }
                    }
                    else if (hitItem && hitGroundDistance > hitItemDistance)
                    {
                        RaycastHit hit;
                        if (Physics.SphereCast(transform.position + orientation.forward.normalized * sphereCastOffset, sphereForDetectingPlayerRadius, orientation.forward, out hit, lightSightDistance, itemLayer))
                        {
                            itemFound(light, itemHit.point);
                            Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.blue, 0.1f);
                        }
                        else if (Physics.SphereCast(transform.position - orientation.forward.normalized * sphereCastOffsett, sphereForDetectingPlayerRadius, -orientation.forward, out hit, lightSightDistance, itemLayer))
                        {
                            Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.cyan, 0.1f);
                        }
                        else if (hitPlayerDistance <= autoSenseDistance)
                        {
                            itemFound(light, itemHit.point);
                            Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.blue, 0.1f);
                        }
                    }
                }
            }
        }
    }

    public void playerFound(Vector3 playerLocation)
    {
        seenPlayer = true;
        targetObject = playerObject;
        timeSinceSeenPlayer = 0;
        lastSeenPlayerLocation = playerLocation;
    }

    public void playerForgot()
    {
        Debug.Log("Forgot Player");
        seenPlayer = false;
        targetObject = null;
    }

    public void itemFound(GameObject obj, Vector3 itemLocation)
    {
        lastSeenItemLocation = itemLocation;
        if (!seenPlayer)
        {
            targetObject = obj;
        }
    }

    public void moveTowards(Vector3 targetPosition)
    {
        navMeshAgent.enabled = true;
        navMeshAgent.destination = targetPosition;
    }

    public void stopMoving()
    {
        navMeshAgent.enabled = false;
    }
}
