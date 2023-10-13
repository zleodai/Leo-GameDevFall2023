using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableScript : MonoBehaviour
{
    void OnCollisionStay(Collision collision)
    {
        GameManager.instance.removeMe(gameObject);
    }
}
