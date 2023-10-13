using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    float timeLeftAlive;
    void Awake()
    {
        timeLeftAlive = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeftAlive -= Time.deltaTime;

        if (timeLeftAlive <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
