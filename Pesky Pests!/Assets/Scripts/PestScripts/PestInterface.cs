using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PestInterface
{
    public enum State
    {
        Idle,
        Patroling,
        Stalking,
        Chasing,
        Attacking,
        Running
    }

    void StateHandeler();
    void TransitionState(State newState);
}
