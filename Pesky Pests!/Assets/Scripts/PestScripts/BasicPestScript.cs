using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPestScript : MonoBehaviour
{
    GameManager gameManager;
    Transform playerTransform;
    public Vector3 orientation;
    public Vector3 playerPosition;
    public Vector3 position;

    private void Awake()
    {
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerMesh").transform;
    }

    private void Update()
    {
        orientation = Quaternion.LookRotation(playerTransform.position - transform.position, playerTransform.up).eulerAngles;
        playerPosition = playerTransform.position;
        position = transform.position;
        transform.rotation = Quaternion.EulerAngles(orientation.x, orientation.y, orientation.z);
    }
}
