using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseEntity : IBrainFSM
{
    [Header("Entity Team")]
    public Team EntityTeam = Team.Unassigned;

    [Header("Entity States")]
    public EntityState[] EntityStates;
    public EntityState FirstEntityState;

    [Header("Components")]
    public Rigidbody2D RgdBdy2D;
    public Animator AnimCtrl;

    [Header("Horizontal Movement")]
    public Vector2 MinMaxMoveSpeed = new Vector2(7f, 15f);
    public float MoveSmoothSpeed = 0.15f;
    public float CurrentSpeed = 7f;

    [Header("Vertical Movement")]
    public float JumpVelocity = 10f;
    public float JumpDelay = 0.25f;
    public float JumpTimer;

    [Header("Rotation")]
    public bool IsFacingRight = true;

    [Header("Ground Check")]
    public float GroundLength = 0.6f;
    public Vector3 ColliderOffset;
    public bool OnGround;

    [Header("Detection")]
    public float DetectRadius = 3f;
    public float DistanceTreshold = 1.5f;
    public Transform Target;

    [Header("Physics 2D")]
    [Range(1, 5)]
    public float FallMultiplier = 2.5f;
    [Range(1, 5)]
    public float LowJumpMultiplier = 2f;

    [Header("Debug")]
    public Vector3 Velocity;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (RgdBdy2D == null || AnimCtrl == null)
            throw new Exception(string.Format("Rigidbody2D or Animator Component unassigned"));

        InitializeFSM();
    }

    private void InitializeFSM()
    {
        //TODO add acronym to __${s}State
        foreach (EntityState s in EntityStates) {
            string strType = $"{s}State";
            Type t = Type.GetType(strType);
            if (t == null)
                throw new Exception(strType + " doesn't implemented yet");
            IState state = (IState)Activator.CreateInstance(t, this, 0);
            this.RegisterState(state);
        }
        string csString = $"{FirstEntityState}State";
        Type cs = Type.GetType(csString);
        ChangeState(cs);
    }

    public void RotateEntity(float xDir)
    {
        if (!IsFacingRight && xDir > 0)
            FlipSprite();
        else if (IsFacingRight && xDir < 0)
            FlipSprite();
    }

    private void FlipSprite()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 flipScale = RgdBdy2D.transform.localScale;
        flipScale.x *= -1;
        RgdBdy2D.transform.localScale = flipScale;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateBrain();

        Vector3 currentPos = RgdBdy2D.transform.position;
        OnGround = Physics2D.Raycast(currentPos + ColliderOffset, 
            Vector2.down, GroundLength, Data.GroundLayer);
    }

    private void FixedUpdate()
    {
        if (JumpTimer > Time.time && OnGround) {
            SendMessageToBrain(ugMessageType.Jump);
        }
        PhysicsData.JumpGravityCalculation(RgdBdy2D, FallMultiplier, LowJumpMultiplier);
    }

    private void OnDrawGizmos()
    {
        if (RgdBdy2D == null)
            return;

        Vector3 currentPos = RgdBdy2D.transform.position;

        //for detection
        if (IsAI) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(currentPos, DetectRadius);
        }

        //for ground
        Gizmos.color = Color.red;
        Gizmos.DrawLine(currentPos + ColliderOffset, currentPos + ColliderOffset + Vector3.down * GroundLength);
    }
}

public enum EntityState
{
    Unassigned,
    Navigation,
    Attack,
}