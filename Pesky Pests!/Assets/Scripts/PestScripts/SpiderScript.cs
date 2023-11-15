using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderScript : MonoBehaviour, PestInterface
{
    public PestInterface.State state;

    private void Start()
    {
        state = PestInterface.State.Idle;
    }

    void PestInterface.StateHandeler()
    {
        switch (state)
        {
            case PestInterface.State.Idle:

                break;
            case PestInterface.State.Patroling:

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

                break;
            case PestInterface.State.Patroling:

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
}
