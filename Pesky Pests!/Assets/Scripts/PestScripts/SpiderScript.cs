using System;
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
    private Transform playerMesh;
    private Transform orientation;
    private NavMeshAgent navMeshAgent;
    public Material dimSpiderEyeMaterial;
    public Material brightSpiderEyeMaterial;

    [Header("Layers")]
    private LayerMask playerLayer;
    private LayerMask itemLayer;
    private LayerMask groundLayer;

    [Header("Checklight/CheckVision")]
    public float lightSightDistance;
    public float sphereForDetectingPlayerRadius;
    public float autoSenseDistance;
    public float sphereCastOffset;
    public float sphereCastOffsett;
    public float forwardVisionSphereCast;
    public float forwardVisionDistance;
    public float generalAwarenessDistance;

    [Header("Memory")]
    public bool onAlert;
    public Vector3[] patrolPoints;
    public bool seenPlayer = false;
    private Vector3 defaultlastSeenLocation = new Vector3(999f, 999f, 990f);
    public Vector3 lastSeenPlayerLocation = new Vector3(999f, 999f, 990f);
    private float timeSinceSeenPlayer = 0;
    public float timeToForgetPlayer = 10;
    public GameObject targetObject = null;
    private Vector3 lastSeenItemLocation = new Vector3(999f, 999f, 999f);
    public float distanceToStalk;
    public float distanceToLastPlayerSpot;
    public float distanceToAttack;
    public int targetPatrolPoint;
    private GameObject[] lightObjects;

    [Header("SpiderStats")]
    public float health;
    public float spiderSpeed;
    public float spiderRunSpeed;
    public float spiderDefaultSpeed;
    public float damagePerAttack;
    public float attackCooldown;

    [Header("Debuffs")]
    public bool onFire;
    public float fireVulnurability;

    [Header("Events")]
    public Dictionary<int, Event> activeEvents;
    public Dictionary<int, float> eventTimers;
    //public Dictionary<Event, bool> atleastOneEventActive;
    private int eventIds = 0;
    public List<int> inactiveEvents;
    public enum Event
    {
        tookDamage,
        death,
        attacking,
        attackCooldown
    }
    private bool stunned;
    private bool attacking;

    public PestInterface.State state;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        orientation = transform.Find("Orientation").transform;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerMesh = playerObject.transform.Find("PlayerMesh").transform;
        playerLayer = LayerMask.GetMask("PlayerLayer");
        itemLayer = LayerMask.GetMask("Item");
        groundLayer = LayerMask.GetMask("GroundLayer");

        makeEyesBright(false);

        lightSightDistance = 100f;
        sphereForDetectingPlayerRadius = 20f;
        autoSenseDistance = 10f;
        sphereCastOffset = 3f;
        sphereCastOffsett = -11f;

        spiderDefaultSpeed = 7f;
        spiderRunSpeed = 10f;
        spiderSpeed = spiderDefaultSpeed;

        health = 100f;

        forwardVisionDistance = 50f;
        forwardVisionSphereCast = 10f;
        generalAwarenessDistance = 10f;

        state = PestInterface.State.Idle;
        gameManager = GameManager.instance;
        if (gameManager == null)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        }

        distanceToStalk = 50f;
        distanceToAttack = 4f;

        damagePerAttack = 20f;
        attackCooldown = 2f;

        onAlert = false;

        onFire = false;
        fireVulnurability = 2.5f;
        stunned = false;

        lightObjects = GameObject.FindGameObjectsWithTag("LightObject");

        //Events

        activeEvents = new Dictionary<int, Event>();
        eventTimers = new Dictionary<int, float>();
        inactiveEvents = new List<int>();
        /*atleastOneEventActive = new Dictionary<Event, bool>();
        var eventTypes = Enum.GetValues(typeof(Event));
        foreach (Event evant in eventTypes)
        {
            atleastOneEventActive[evant] = false;
        }*/

        GameObject[] patrolPointObjects = GameObject.FindGameObjectsWithTag("SpiderPatrolPoint");
        patrolPoints = new Vector3[patrolPointObjects.Length];
        int index;
        foreach (GameObject obj in patrolPointObjects)
        {
            index = UnityEngine.Random.Range(0, patrolPointObjects.Length);
            patrolPoints[index] = obj.transform.position;
        }

        targetPatrolPoint = 0;
    }

    private void Update()
    {
        distanceToLastPlayerSpot = Vector3.Distance(transform.position, lastSeenPlayerLocation);
        navMeshAgent.speed = spiderSpeed;
        if (gameManager == null)
        {
            Debug.Log("Game manager not found??");
        }
        switch (gameManager.gameState)
        {
            case GameManager.GameState.GAMEPLAY:
                if (state == PestInterface.State.Paused)
                {
                    TransitionState(PestInterface.State.Idle);
                }
                StateHandeler();
                EventChecker();
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
        if (state != PestInterface.State.Paused)
        {
            fireCheck();
            if (health <= 0) 
            {
                print(state);
                print(health);
                deathEvent();
            }
        }

        switch (state)
        {
            case PestInterface.State.Paused:
                spiderSpeed = 0;
                break;
            case PestInterface.State.Idle:
                checkLights();
                checkVision();
                spiderSpeed = spiderDefaultSpeed;
                if (!seenPlayer)
                {
                    TransitionState(PestInterface.State.Patroling);
                }
                else
                {
                    if (distanceToLastPlayerSpot > distanceToStalk) TransitionState(PestInterface.State.Stalking);
                    else TransitionState(PestInterface.State.Chasing);
                }
                break;
            case PestInterface.State.Patroling:
                checkLights();
                checkVision();
                spiderSpeed = spiderDefaultSpeed;
                if (seenPlayer || targetObject != null)
                {
                    if (distanceToLastPlayerSpot > distanceToStalk) TransitionState(PestInterface.State.Stalking);
                    else TransitionState(PestInterface.State.Chasing);
                }
                else
                {
                    Vector3 targetPosition = patrolPoints[targetPatrolPoint];
                    //Debug.LogFormat("Target: {0},  Distance: {1}", targetPosition, Vector3.Distance(targetPosition, transform.position));
                    if (Vector3.Distance(targetPosition, transform.position) < 5f)
                    {
                        targetPatrolPoint++;
                        while (patrolPoints[targetPatrolPoint] != new Vector3(0f, 0f, 0f))
                        {
                            targetPatrolPoint++;
                            if (targetPatrolPoint >= patrolPoints.Length)
                            {
                                targetPatrolPoint = 0;
                            }
                        }

                        targetPosition = patrolPoints[targetPatrolPoint];
                    }
                    moveTowards(targetPosition);
                    navMeshAgent.enabled = true;
                }
                break;
            case PestInterface.State.Stalking:
                checkLights();
                checkVision();
                moveTowards(lastSeenPlayerLocation);
                spiderSpeed = spiderDefaultSpeed;
                if (distanceToLastPlayerSpot < distanceToStalk) TransitionState(PestInterface.State.Chasing);
                break;
            case PestInterface.State.Chasing:
                checkLights();
                checkVision();
                if (stunned)
                {
                    spiderSpeed = spiderDefaultSpeed;
                } else
                {
                    spiderSpeed = spiderRunSpeed;
                }
                moveTowards(lastSeenPlayerLocation);
                if (Vector3.Distance(transform.position, playerMesh.position) < distanceToAttack)
                {
                    TransitionState(PestInterface.State.Attacking);
                }
                break;
            case PestInterface.State.Attacking:
                checkLights();
                checkVision();
                if (stunned)
                {
                    spiderSpeed = spiderDefaultSpeed;
                }
                else
                {
                    spiderSpeed = spiderRunSpeed;
                }
                moveTowards(lastSeenPlayerLocation);
                if (Vector3.Distance(transform.position, playerMesh.position) > distanceToAttack)
                {
                    TransitionState(PestInterface.State.Chasing);
                } else
                {
                    if (!attacking)
                    {
                        AddEvent(Event.attacking, 0.5f);
                    }
                }
                break;
            case PestInterface.State.Running:

                break;
            case PestInterface.State.Dead:

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
                makeEyesBright(false);
                break;
            case PestInterface.State.Patroling:
                state = PestInterface.State.Patroling;
                makeEyesBright(false);
                break;
            case PestInterface.State.Stalking:
                state = PestInterface.State.Stalking;
                makeEyesBright(true);
                break;
            case PestInterface.State.Chasing:
                state = PestInterface.State.Chasing;
                makeEyesBright(true);
                break;
            case PestInterface.State.Attacking:
                state = PestInterface.State.Attacking;
                makeEyesBright(true);
                break;
            case PestInterface.State.Running:

                break;
            case PestInterface.State.Dead:
                state = PestInterface.State.Dead;
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        seenPlayer = true;
        lastSeenPlayerLocation = playerMesh.position;
        AddEvent(Event.tookDamage, 0.1f);
    }

    public void AddDebuff(PestInterface.Debuff debuff)
    {
        switch (debuff)
        {
            case PestInterface.Debuff.OnFire:
                onFire = true;
                break;
        }
    }

    private void checkLights()
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
                            //Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.red, 0.1f);
                        }
                        else if (Physics.SphereCast(transform.position - orientation.forward.normalized * sphereCastOffsett, sphereForDetectingPlayerRadius, -orientation.forward, out hit, lightSightDistance, playerLayer))
                        {
                            //Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.yellow, 0.1f);
                        }
                        else if (hitPlayerDistance <= autoSenseDistance)
                        {
                            playerFound(playerMesh.position);
                            //Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.green, 0.1f);
                        }
                        if (hitPlayerDistance <= generalAwarenessDistance)
                        {
                            playerFound(playerMesh.position);
                            //Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.gray, 0.1f);
                        }
                    }
                    else if (hitItem && hitGroundDistance > hitItemDistance)
                    {
                        RaycastHit hit;
                        if (Physics.SphereCast(transform.position + orientation.forward.normalized * sphereCastOffset, sphereForDetectingPlayerRadius, orientation.forward, out hit, lightSightDistance, itemLayer))
                        {
                            itemFound(light, itemHit.point);
                            //Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.blue, 0.1f);
                        }
                        else if (hitPlayerDistance <= autoSenseDistance)
                        {
                            itemFound(light, light.transform.position);
                            //Debug.DrawRay(transform.position, vectorTowardsLight.normalized * lightSightDistance, Color.gray, 0.1f);
                        }
                    }
                }
            }
        }
    }

    private void checkVision()
    {
        Vector3 vectorTowardsPlayer = playerMesh.position - transform.position;
        RaycastHit sphereCastPlayerHit;
        bool hitPlayer = false;

        if (Physics.SphereCast(transform.position + orientation.forward.normalized * -2, forwardVisionSphereCast, orientation.forward, out sphereCastPlayerHit, forwardVisionDistance, playerLayer))
        {
            hitPlayer = true;
        }

        RaycastHit groundHit;
        Physics.Raycast(transform.position, vectorTowardsPlayer.normalized, out groundHit, forwardVisionDistance, groundLayer);

        RaycastHit playerHit;
        Physics.Raycast(transform.position, vectorTowardsPlayer.normalized, out playerHit, forwardVisionDistance, playerLayer);

        Vector3 defaultPoint = new Vector3(0f, 0f, 0f);
        float hitGroundDistance = Vector3.Distance(transform.position, groundHit.point);
        float hitplayerSphereCastDistance = Vector3.Distance(transform.position, sphereCastPlayerHit.point);
        float hitPlayerDistance = Vector3.Distance(transform.position, playerHit.point);
        if (groundHit.point == defaultPoint) hitGroundDistance = 9999;
        if (sphereCastPlayerHit.point == defaultPoint) hitplayerSphereCastDistance = 9999;
        if (playerHit.point == defaultPoint) hitPlayerDistance = 9999;

        if (hitPlayer && hitGroundDistance > hitplayerSphereCastDistance)
        {
            playerFound(sphereCastPlayerHit.point);
            //Debug.DrawRay(transform.position, vectorTowardsPlayer.normalized * forwardVisionDistance, Color.white, 0.1f);
        }
        else if (Vector3.Distance(playerMesh.position, transform.position) <= generalAwarenessDistance && hitGroundDistance > hitPlayerDistance)
        {
            playerFound(playerMesh.position);
            //Debug.DrawRay(transform.position, vectorTowardsPlayer.normalized * forwardVisionDistance, Color.gray, 0.1f);
        }
    }

    public void playerFound(Vector3 playerLocation)
    {
        seenPlayer = true;
        targetObject = playerMesh.gameObject;
        timeSinceSeenPlayer = 0;
        lastSeenPlayerLocation = playerLocation;
        distanceToLastPlayerSpot = Vector3.Distance(transform.position, lastSeenPlayerLocation);
        if (!onAlert)
        {
            alertSpider();
        }
    }

    public void playerForgot()
    {
        seenPlayer = false;
        targetObject = null;
        TransitionState(PestInterface.State.Idle);
        makeEyesBright(false);
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

    private void makeEyesBright(bool bright)
    {
        if (bright)
        {
        }
        else
        {
        }
        //Depricated
    }

    private void alertSpider()
    {
        onAlert = true;
        lightSightDistance = 150f;
        sphereForDetectingPlayerRadius = 30f;
        autoSenseDistance = 20f;
        sphereCastOffset = 5f;
        sphereCastOffsett = -11f;

        forwardVisionDistance = 80f;
        forwardVisionSphereCast = 15f;
        generalAwarenessDistance = 15f;
    }

    private void enableAllFire(bool enable)
    {
        if (enable)
        {
        } else
        {
        }
        //Depricated
    }

    private void fireCheck()
    {
        if (onFire)
        {
            health -= fireVulnurability * Time.deltaTime;
            enableAllFire(true);
        }
        else
        {
            enableAllFire(false);
        }
    }

    private void deathEvent()
    {

        makeEyesBright(false);
        stopMoving();

        //Death Ragdoll
        SpiderProceduralAnimation spd = transform.Find("Spider").GetComponent<SpiderProceduralAnimation>();
        Transform[] legTargets = spd.legTargets;
        Destroy(spd);
        foreach (Transform legTarget in legTargets)
        {

            legTarget.position = legTarget.position += new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f));
        }

        SkinnedMeshRenderer eyes = transform.Find("Spider").Find("SpiderEyes").gameObject.GetComponent<SkinnedMeshRenderer>();
        eyes.enabled = false;

        TransitionState(PestInterface.State.Dead);
        Destroy(this);
    }
    // Fix this pls
    private void EventChecker()
    {
        foreach (int id in activeEvents.Keys)
        {
            eventTimers[id] -= Time.deltaTime;
            if (eventTimers[id] <= 0)
            {
                EventHandeler(id, false);
                inactiveEvents.Add(id);
            }
            else
            {
                EventHandeler(id, true);
            }
        }
        foreach (int id in inactiveEvents)
        {
            activeEvents.Remove(id);
            eventTimers.Remove(id);
        }
        inactiveEvents.Clear();
    }
    private void EventHandeler(int id, bool active)
    {
        switch (activeEvents[id], active)
        {
            case (Event.tookDamage, true):
                if (health > 0)
                {
                    stunned = true;
                }
                break;
            case (Event.tookDamage, false):
                stunned = false;
                break;
            case (Event.attacking, true):

                if (health > 0 && attacking == false)
                {
                    attacking = true;
                }
                break;
            case (Event.attacking, false):
                if (health > 0 && attacking == true)
                {
                    attacking = false;
                    playerObject.GetComponent<PlayerControllerScript>().takeDamage(damagePerAttack);
                    Vector3 vectorTowardsPlayer = playerMesh.position - transform.position;
                    Debug.DrawRay(transform.position, vectorTowardsPlayer.normalized * forwardVisionDistance, Color.red, 1f);

                    AddEvent(Event.attackCooldown, attackCooldown);
                }
                break;
            default:
                Debug.LogFormat("Error: {0} not added to EventHandeler", activeEvents[id]);
                break; 
        }
    }
    private void AddEvent(Event evant, float time)
    {
        activeEvents[eventIds] = evant;
        eventTimers[eventIds] = time;
        eventIds++;
    }
}
