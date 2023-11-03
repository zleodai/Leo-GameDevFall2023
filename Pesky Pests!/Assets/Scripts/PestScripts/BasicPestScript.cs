using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPestScript : MonoBehaviour
{
    [Header("Refrences")]
    GameManager gameManager;
    Transform playerTransform;
    Transform playerOrientation;

    [Header("Public Data")]
    public Vector3 orientation;
    public Vector3 playerPosition;
    public Vector3 position;
    public float distanceFromPlayer;

    public State state;
    public enum State
    {
        Idle,
        Patroling,
        Stalking,
        Chasing,
        Attacking,
        Running
    }

    [Header("Private Data")]
    private float placeHolder;

    private void Awake()
    {
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerMesh").transform;
        playerOrientation = GameObject.FindGameObjectWithTag("Player").transform.Find("Orientation").transform;

        state = State.Idle;
    }

    private void Update()
    {
        orientation = Quaternion.LookRotation(playerTransform.position - transform.position, playerOrientation.up).eulerAngles;
        playerPosition = playerTransform.position;
        position = transform.position;
        transform.rotation = Quaternion.Euler(0f, orientation.y + 90f, orientation.z);

        distanceFromPlayer = Vector3.Distance(playerPosition, position);

        StateHandeler();
    }

    private void StateHandeler()
    {
        switch (state)
        {
            case State.Idle:
                
                break;
            case State.Patroling:
                
                break;
            case State.Stalking:
                
                break;
            case State.Chasing:
                
                break;
            case State.Attacking:
                
                break;
            case State.Running:
                
                break;
        }
    }

    public void TransitionState(State newState)
    {
        switch (newState)
        {
            case State.Idle:
                
                break;
            case State.Patroling:
                
                break;
            case State.Stalking:
                
                break;
            case State.Chasing:
                
                break;
            case State.Attacking:
                
                break;
            case State.Running:
                
                break;
        }
    }
}
