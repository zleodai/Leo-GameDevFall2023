using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PestInterface
{
    public enum State
    {
        Paused,
        Idle,
        Patroling,
        Stalking,
        Chasing,
        Attacking,
        Running,
        Dead
    }

    public enum Debuff
    {
        OnFire
    }

    void StateHandeler();
    void TransitionState(State newState);
    void TakeDamage(float damage);
    void AddDebuff(Debuff debuff);
}
