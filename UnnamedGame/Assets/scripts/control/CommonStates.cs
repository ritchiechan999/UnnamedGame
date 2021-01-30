using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationState : IBaseState<BaseEntity>
{
    public NavigationState(BaseEntity brain, int initConstruct) : base(brain) { }

    public override void OnReceiveMessage(ugMessageType msgType, object[] args)
    {
        switch (msgType) {
            case ugMessageType.Move:
                if (!Entity.IsAI) {
                    float horizontal = (float)args[0];
                    MoveEntity(horizontal);
                    Entity.RotateEntity(horizontal);
                    if (Entity.OnGround) {
                        //MovementAnimation(horizontal);
                    }
                    return;
                }

                if (Entity.IsAI) {

                    Transform target = (Transform)args[0];
                    if (Vector2.Distance(Entity.RgdBdy2D.position, target.position) < Entity.DistanceTreshold) {
                        //TODO send message to do attack state or something
                        return;
                    }
                    Vector2 direction = ((Vector2)target.position - Entity.RgdBdy2D.position).normalized;
                    direction.y = 0;
                    MoveEntity(direction);
                    Entity.RotateEntity(direction.x);
                }
                break;
            case ugMessageType.Jump:
                //Entity.AnimCtrl.SetTrigger("jump");
                //Entity.AnimCtrl.SetFloat("nav_speed", 0);
                Entity.Velocity = new Vector2(Entity.Velocity.x, 0);
                Entity.RgdBdy2D.AddForce(Vector2.up * Entity.JumpVelocity, ForceMode2D.Impulse);
                Entity.JumpTimer = 0;
                break;
            case ugMessageType.Attack:
                //go to attack state
                //Entity.ChangeState(typeof(tdAttackState), args);
                break;
            case ugMessageType.Flinch:
                //chance to change state or idk to be discussed
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
        //Entity.AnimCtrl.SetFloat("yVelocity", Entity.RgdBdy2D.velocity.y);
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
        float sprintSpeed = Mathf.Lerp(Entity.MinMaxMoveSpeed.y, Entity.MinMaxMoveSpeed.x, Entity.MoveSmoothSpeed);
        float runSpeed = Mathf.Lerp(Entity.MinMaxMoveSpeed.x, Entity.MinMaxMoveSpeed.y, Entity.MoveSmoothSpeed);
        Entity.CurrentSpeed = isSprinting && Entity.OnGround ? sprintSpeed : runSpeed;
        Entity.RgdBdy2D.velocity = new Vector2(xDir * Entity.CurrentSpeed, Entity.RgdBdy2D.velocity.y);
    }

    private void MoveEntity(Vector2 direction)
    {
        Entity.RgdBdy2D.velocity = direction * Entity.MinMaxMoveSpeed.x;
    }
}

//TODO: revise State
public class AttackState : IBaseState<BaseEntity>
{
    public AttackState(BaseEntity brain, int initConstruct) : base(brain) { }
    public override void OnReceiveMessage(ugMessageType msgType, object[] args)
    {
        switch (msgType) {
            case ugMessageType.Attack:
                //tdAttack currentAtk = (tdAttack)args[0];
                //Entity.AnimCtrl.SetInteger("anim_state", (int)currentAtk.AnimState);
                break;
            case ugMessageType.Flinch:
                break;
        }
    }

    public override void OnStateEnter(object[] args)
    {
        if (args != null) {
            //tdAttack currentAtk = (tdAttack)args[0];
            //Entity.AnimCtrl.SetInteger("anim_state", (int)currentAtk.AnimState);
        }
    }

    public override void OnStateExit(object[] args)
    {
    }

    public override void OnStateUpdate()
    {
    }
}