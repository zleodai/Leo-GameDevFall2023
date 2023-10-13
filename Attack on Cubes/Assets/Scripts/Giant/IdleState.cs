using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateMachineBehaviour
{
    float timer;
    Transform player;
    public int id;
    public bool dead;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //Not Needed but just as a saftey precaution
        animator.SetBool("isAttacking", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("isPatroling", false);
        animator.SetBool("beenHit", false);

        id = Random.Range(0, 1000);
        animator.SetInteger("id", id);

        EnemyManager.instance.addEnemy(this);
        dead = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (timer > 4f)
        {
            animator.SetBool("isPatroling", true);
        }
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance > animator.GetFloat("attackRange")) animator.SetBool("isAttacking", true);
        else if (distance > animator.GetFloat("chaseRange")) animator.SetBool("isChasing", true);

        animator.SetInteger("id", id);
        animator.SetBool("dead", dead);
    }

    public int regenerateId()
    {
        int newId = Random.Range(0, 1000);
        id = newId;
        return newId;
    }

    public void setDeath()
    {
        dead = true;
    }
}
