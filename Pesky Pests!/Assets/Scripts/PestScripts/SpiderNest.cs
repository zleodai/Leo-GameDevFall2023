using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderNest : MonoBehaviour, PestInterface
{
    private ParticleSystem[] fires;
    private GameManager gameManager;
    private float counter;
    void Start()
    {
        gameManager = GameManager.instance;
        fires = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem fire in fires)
        {
            fire.enableEmission = false;
        }
        counter = 404f;
    }
    void Update()
    {
        if (counter != 404)
        {
            counter += Time.deltaTime;
            if (counter > 5)
            {
                gameManager.winStateMet();
            }
        }
    }

    void PestInterface.StateHandeler()
    {
        throw new System.NotImplementedException();
    }
 
    public void TakeDamage(float damage)
    {
        return;
    }

    public void AddDebuff(PestInterface.Debuff debuff)
    {
        if (debuff == PestInterface.Debuff.OnFire)
        {
            foreach (ParticleSystem fire in fires)
            {
                fire.enableEmission = true;
                counter = 0f;
            }
        }
    }

    void PestInterface.TransitionState(PestInterface.State newState)
    {
        throw new System.NotImplementedException();
    }
}
