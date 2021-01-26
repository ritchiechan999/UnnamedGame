using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseEntity : IBrainFSM
{
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
    public float JumpSpeed = 15f;
    public float JumpDelay = 0.25f;
    public float JumpTimer;

    [Header("Rotation")]
    public bool IsFacingRight = true;
    private Quaternion lastRotation;
    public float RotateSmoothSpeed = 0.15f;

    [Header("Collision")]
    public float GroundLength = 0.6f;
    public Vector3 ColliderOffset;
    public bool OnGround;

    [Header("Physics")]
    [Range(1, 10)]
    public float CustomGravity = 1f;
    public float FallMultiplier = 5f;
    public float Drag = 4f;

    [Header("Debug")]
    public Vector3 Velocity;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (RgdBdy2D == null || AnimCtrl == null)
            throw new Exception(string.Format("Rigidbody2D or Animator Component unassigned"));
        //RgdBdy2D = this.GetComponent<Rigidbody2D>();
        //AnimCtrl = this.GetComponent<Animator>();

        InitializeFSM();

        //_lastRotation = new Quaternion(0, 0.7f, 0, 0.7f);
        //RgdBdy2D.useGravity = false;
        //lastRotation = new Quaternion(0, 0.7f, 0, 0.7f);
        RgdBdy2D.gravityScale = 0;
    }

    void InitializeFSM()
    {
        //TODO add
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

    //TODO: 2D rotation only
    public void RotateEntity(float xDir)
    {
        if (xDir == 0) {
            this.transform.rotation = lastRotation;
            return;
        }
        Quaternion rotation = Quaternion.Slerp(this.transform.rotation,
                                Quaternion.LookRotation(new Vector3(0, xDir, 0)), RotateSmoothSpeed);
        transform.rotation = rotation;
        lastRotation = rotation;
        IsFacingRight = transform.rotation.y > 0;

        //if (xDir == 0) {
        //    this.transform.localScale = Vector3.one;
        //    return;
        //}

        //Vector3 lookScale = Vector3.one;
        //lookScale.x = (int)xDir;
        //this.transform.localScale = lookScale;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateBrain();
    }

    private void FixedUpdate()
    {
        if (JumpTimer > Time.time && OnGround) {
            SendMessageToBrain(ugMessageType.Jump);
        }

        PhysicsData.ModifyPhysics(OnGround, RgdBdy2D, CustomGravity, Drag, FallMultiplier, Velocity.x);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 currentPos = this.transform.position;
        Gizmos.DrawLine(currentPos + ColliderOffset, currentPos + ColliderOffset + Vector3.down * GroundLength);
    }
}

public enum EntityState
{
    Unassigned,
    Navigation,
    Attack,
}