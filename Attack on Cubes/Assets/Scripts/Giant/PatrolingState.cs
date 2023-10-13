using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolingState : StateMachineBehaviour
{
    float timer;
    Transform player;
    GameObject titan;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        titan = EnemyTitan.instance.gameObject;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (timer > 10f)
        {
            animator.SetBool("isPatroling", false);
        }
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance > animator.GetFloat("attackRange")) animator.SetBool("isChasing", true);
        else if (distance > animator.GetFloat("chaseRange")) animator.SetBool("isChasing", true);

        titan.transform.position = Vector3.MoveTowards(titan.transform.position, player.position, 1f * Time.deltaTime);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isPatroling", false);
    }
}
