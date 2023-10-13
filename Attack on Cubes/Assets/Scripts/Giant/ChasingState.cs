using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingState : StateMachineBehaviour
{
    Transform player;
    GameObject titan;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        titan = EnemyTitan.instance.gameObject;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance > animator.GetFloat("attackRange")) animator.SetBool("isChasing", true);
        else if (distance > animator.GetFloat("chaseRange")) animator.SetBool("isPatroling", true);

        titan.transform.position = Vector3.MoveTowards(titan.transform.position, player.position, 31f * Time.deltaTime);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isPatroling", false);
    }
}
