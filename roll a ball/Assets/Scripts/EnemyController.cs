using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody rb;
    public float ForceMult;

    void Start()
    {
        playerController = PlayerController.instance;
        rb = GetComponent<Rigidbody>();
        ForceMult = 500f;
    }

    private Vector2 movementHelper(float yAngle, Vector2 inputVector)
    {
        Vector2 outputVector = new Vector2();

        float hyp = Mathf.Sqrt(Mathf.Pow(inputVector.y, 2) + Mathf.Pow(inputVector.x, 2));
        float inputAngle;

        //Idk why something to do with quadrants or someshit but it works for now
        if (inputVector.x == 0)
        {
            inputAngle = Mathf.Asin(inputVector.y / hyp) * 57.2958f;
        }
        else
        {
            inputAngle = Mathf.Acos(inputVector.x / hyp) * 57.2958f;
        }

        float forceAngle = yAngle + inputAngle;

        //Debug.Log("Angle: " + forceAngle);
        //Debug.Log("inputY: " + inputVector.y + ", inputX: " + inputVector.x);

        outputVector.y = hyp * Mathf.Sin(forceAngle * 0.0174533f);
        outputVector.x = hyp * Mathf.Cos(forceAngle * 0.0174533f);

        //Debug.Log("Sin: " + Mathf.Sin(forceAngle * 0.0174533f) + ", Cos: " + Mathf.Cos(forceAngle * 0.0174533f));
        //Debug.Log("x Force: " + outputVector.x + ", z Force: " + outputVector.y);

        if (inputVector.y == 0)
        {
            outputVector.x = -outputVector.x;
        }
        else if (inputVector.x == 0)
        {
            outputVector.y = -outputVector.y;
        }

        return outputVector;
    }

    void FixedUpdate()  
    {
        //Randomly moves towards the player
        if (Random.Range(0.0f, 100.0f) > 99.0f)
        {
            //Creating a vector based on player location and finding the diffrence between that and this objects location
            Vector3 offset = playerController.gameObject.transform.position - transform.position;
            rb.AddForce(Vector3.Normalize(offset) * ForceMult, ForceMode.Impulse);
        }
    }
}
