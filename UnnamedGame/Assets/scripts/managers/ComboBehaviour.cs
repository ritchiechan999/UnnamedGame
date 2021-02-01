﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboBehaviour : StateMachineBehaviour
{
    public tdComboAttack animStateInt = tdComboAttack.Default;
    public KeyCode inputKey;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ComboManager.instance.canReceiveInput = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ComboManager.instance.inputReceived)
        {
            if (animStateInt != tdComboAttack.Default 
                && ComboManager.instance.currentInputKey == inputKey)
            {
                animator.SetInteger("animState", (int)animStateInt);
                ComboManager.instance.InputManager();
                ComboManager.instance.inputReceived = false;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("animState", 0);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
