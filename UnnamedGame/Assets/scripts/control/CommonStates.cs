﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All states must have prefix and suffix of ass and State
/// eg. assNavigationState (ass*Name*State)
/// </summary>
public enum assEntityState
{
    Unassigned,
    Navigation,
    Attack,
    FirstPhase,
    FinalPhase,
    Thinking,

}

public class assNavigationState : IBaseState<assBaseEntity>
{
    public assNavigationState(assBaseEntity brain, int initConstruct) : base(brain) { }

    public override void OnReceiveMessage(assMessageType msgType, object[] args)
    {
        switch (msgType) {
            case assMessageType.Move:
                if (!Entity.IsAI) {
                    float horizontal = (float)args[0];
                    MoveEntity(horizontal);
                    Entity.RotateEntity(horizontal);
                    if (Entity.OnGround) {
                        //float horAbsValue = Mathf.Abs(horizontal);
                        MovementAnimation(horizontal);
                    }
                } else if (!Entity.IsAIBoss) {
                    Transform target = (Transform)args[0];
                    if (Vector2.Distance(Entity.RgdBdy2D.position, target.position) < Entity.DistanceTreshold) {
                        //TODO send message to do attack state or something
                        Entity.SendMessageToBrain(assMessageType.Attack, target);
                        return;
                    }
                    Vector2 direction = ((Vector2)target.position - Entity.RgdBdy2D.position).normalized;
                    direction.y = 0;
                    MoveEntity(direction);
                    Entity.RotateEntity(direction.x);
                }
                break;
            case assMessageType.Jump:
                Entity.AnimCtrl.SetFloat("nav_speed", 0);
                Entity.AnimCtrl.SetTrigger("jump");
                Entity.Velocity = new Vector2(Entity.Velocity.x, 0);
                Entity.RgdBdy2D.AddForce(Vector2.up * Entity.JumpVelocity, ForceMode2D.Impulse);
                Entity.JumpTimer = 0;
                break;
            case assMessageType.Attack:
                Entity.ChangeState(typeof(assAttackState), args);
                break;
            case assMessageType.Flinch:
                //chance to change state or idk to be discussed
                break;
            case assMessageType.FirstPhaseActivate:
                Entity.ChangeState(typeof(assFirstPhaseState), args);
                break;
            case assMessageType.FinalPhaseActivate:
                Entity.ChangeState(typeof(assFinalPhaseState), args);
                break;
            default:
                break;
        }
    }

    private bool randomMove;
    private Vector3 randLocation;

    public override void OnStateEnter(object[] args)
    {
        randomMove = (bool)args[0];
        if (randomMove) {
            //initial position when spawned. find other better alternative
            Vector2 randInsideCircle = Random.insideUnitCircle;
            randLocation = randInsideCircle * Entity.randomTravelThreshold + (Vector2)Entity.InitialPosition;
            Debug.Log("location: " + randLocation);
        }
    }

    public override void OnStateExit(object[] args)
    {
        //Entity.AIThinkingNextState(Entity.AIType, Entity.ShortOffsetThinkResetTime);
        randomMove = false;
    }

    public override void OnStateUpdate()
    {
        Entity.AnimCtrl.SetFloat("yVelocity", Entity.RgdBdy2D.velocity.y);
        //Entity.AnimCtrl.SetBool("on_ground", Entity.OnGround);

        if (randomMove) {
            if (Vector2.Distance(Entity.RgdBdy2D.position, randLocation) < Entity.DistanceTreshold) {
                Entity.AIThinkingNextState(Entity.AIType, Entity.ShortOffsetThinkResetTime);
                return;
            }
            Entity.RgdBdy2D.position = Vector2.MoveTowards(Entity.RgdBdy2D.position, randLocation, 
                Entity.MinMaxMoveSpeed.x * Time.deltaTime);

            Vector2 direction = ((Vector2)randLocation - Entity.RgdBdy2D.position).normalized;
            Entity.RotateEntity(direction.x);
        }

    }

    public float SprintDownSpeed = 0.1f;
    private float sprintSpeed;
    public float SprintTimeTreshold = 1f;
    private float sprintTime;
    private bool isSprinting;

    void MovementAnimation(float xDir)
    {
        float moveSpeed = Mathf.Abs(xDir);
        float maxSpeed = Entity.AnimCtrl.GetFloat("nav_speed");
        if (maxSpeed >= 1 && moveSpeed == 1) {
            if (sprintTime < SprintTimeTreshold) {
                sprintTime += Time.deltaTime;
            } else {
                isSprinting = true;
                sprintSpeed += Time.deltaTime;
            }
        } else {
            isSprinting = false;
            sprintSpeed -= SprintDownSpeed / Time.deltaTime;
            sprintTime = 0;
        }
        sprintTime = Mathf.Clamp(sprintTime, 0, SprintTimeTreshold);
        sprintSpeed = Mathf.Clamp01(sprintSpeed);
        moveSpeed += sprintSpeed;
        Entity.AnimCtrl.SetFloat("nav_speed", moveSpeed);
    }

    //TODO decide later where to put below function
    private void MoveEntity(float xDir)
    {
        //if has sprint
        //float sprintSpeed = Mathf.Lerp(Entity.MinMaxMoveSpeed.y, Entity.MinMaxMoveSpeed.x, Entity.MoveSmoothSpeed);
        //float runSpeed = Mathf.Lerp(Entity.MinMaxMoveSpeed.x, Entity.MinMaxMoveSpeed.y, Entity.MoveSmoothSpeed);
        //Entity.CurrentSpeed = isSprinting && Entity.OnGround ? sprintSpeed : runSpeed;
        //Entity.RgdBdy2D.velocity = new Vector2(xDir * Entity.CurrentSpeed, Entity.RgdBdy2D.velocity.y);

        //run only
        float currentRuntSpeed = Mathf.Lerp(Entity.MinMaxMoveSpeed.x, Entity.MinMaxMoveSpeed.y, Entity.MoveSmoothSpeed);
        Entity.RgdBdy2D.velocity = new Vector2(xDir * currentRuntSpeed, Entity.RgdBdy2D.velocity.y);
    }

    private void MoveEntity(Vector2 direction)
    {
        Entity.RgdBdy2D.velocity = direction * Entity.MinMaxMoveSpeed.x;
    }
}
public class assThinkingState : IBaseState<assBaseEntity>
{
    public assThinkingState(assBaseEntity brain, int initConstruct) : base(brain) { }
    public override void OnReceiveMessage(assMessageType msgType, object[] args)
    {
        //switch (msgType) {
        //    case assMessageType.Think:
        //        Entity.ThinkingTime = (float)args[0];
        //        break;
        //    default:
        //        break;
        //}
    }

    public override void OnStateEnter(object[] args)
    {

    }

    public override void OnStateExit(object[] args)
    {
    }

    public override void OnStateUpdate()
    {
        Entity.ThinkingTime -= Time.deltaTime;
        if (Entity.ThinkingTime <= 0) {
            ThinkProcess();
        }
    }

    private void ThinkProcess()
    {
        //to optimize to think equally between attacks and phase only.
        assEntityState randNextAction = Entity.EntityStates.GetRandom();
        switch (randNextAction) {
            case assEntityState.Navigation:
                Debug.Log("Transition to nav state");
                bool randomMove = true;
                Entity.ChangeState(typeof(assNavigationState), randomMove);
                break;
            case assEntityState.Attack:
                Debug.Log("Transition to atk state");
                Entity.ChangeState(typeof(assAttackState), Entity.Target);
                break;
            case assEntityState.FirstPhase:
                Debug.Log("think again");
                ThinkProcess();
                break;
            case assEntityState.FinalPhase:
                Debug.Log("think again");
                ThinkProcess();
                break;
            case assEntityState.Thinking:
                Debug.Log("think again");
                ThinkProcess();
                break;
        }
    }
}
public class assAttackState : IBaseState<assBaseEntity>
{
    //timer to when the attack lasts
    private float attackDuration = 2f;
    private float meleeRange = 4f;

    public assAttackState(assBaseEntity brain, int initConstruct) : base(brain) { }
    public override void OnReceiveMessage(assMessageType msgType, object[] args)
    {
        switch (msgType) {
            case assMessageType.Attack:
                //tdAttack currentAtk = (tdAttack)args[0];
                //Entity.AnimCtrl.SetInteger("anim_state", (int)currentAtk.AnimState);

                //TODO if boss AI check if melee or range

                break;
            case assMessageType.Flinch:
                break;
            case assMessageType.FirstPhaseActivate:
                Entity.ChangeState(typeof(assFirstPhaseState), args);
                break;
            case assMessageType.FinalPhaseActivate:
                Entity.ChangeState(typeof(assFinalPhaseState), args);
                break;
        }
    }

    public override void OnStateEnter(object[] args)
    {
        if (args != null) {
            //tdAttack currentAtk = (tdAttack)args[0];
            //Entity.AnimCtrl.SetInteger("anim_state", (int)currentAtk.AnimState);
        }

        //prototype TODO: to improve
        if (Entity.IsAI) {
            Transform target = (Transform)args[0];

            if (Vector2.Distance(Entity.RgdBdy2D.position, target.position) < meleeRange) {
                Debug.Log("Normal Melee Atk");
                attackDuration = 1f;
            } else {
                Debug.Log("Normal Range Atk");
                attackDuration = 2f;
            }
        }
    }

    public override void OnStateExit(object[] args)
    {
    }

    public override void OnStateUpdate()
    {
        attackDuration -= Time.deltaTime;
        if (attackDuration <= 0) {
            //Entity.ChangeState(typeof(assNavigationState));
            Entity.AIThinkingNextState(Entity.AIType, Entity.MediumOffsetThinkReset);
        }
    }
}
public class assFirstPhaseState : IBaseState<assBaseEntity>
{
    public assFirstPhaseState(assBaseEntity brain, int initConstruct) : base(brain) { }

    public override void OnReceiveMessage(assMessageType msgType, object[] args)
    {
        switch (msgType) {
            case assMessageType.FinalPhaseActivate:
                Entity.ChangeState(typeof(assFinalPhaseState), args);
                break;
        }
    }

    public override void OnStateEnter(object[] args)
    {

    }

    public override void OnStateExit(object[] args)
    {

    }

    public override void OnStateUpdate()
    {

    }
}
public class assFinalPhaseState : IBaseState<assBaseEntity>
{
    public assFinalPhaseState(assBaseEntity brain, int initConstruct) : base(brain) { }

    public override void OnReceiveMessage(assMessageType msgType, object[] args)
    {

    }

    public override void OnStateEnter(object[] args)
    {

    }

    public override void OnStateExit(object[] args)
    {

    }

    public override void OnStateUpdate()
    {

    }
}