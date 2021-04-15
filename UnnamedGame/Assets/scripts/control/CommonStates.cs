using System.Collections;
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
    FinalPhase
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
                    return;
                }

                if (Entity.IsAI) {
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

    public override void OnStateEnter(object[] args)
    {
    }

    public override void OnStateExit(object[] args)
    {
    }

    public override void OnStateUpdate()
    {
        Entity.AnimCtrl.SetFloat("yVelocity", Entity.RgdBdy2D.velocity.y);
        //Entity.AnimCtrl.SetBool("on_ground", Entity.OnGround);
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

        //prototype TODO: to improved
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
            Entity.ChangeState(typeof(assNavigationState));
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