using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class assBaseEntity : IBrainFSM, assIHealthDamageHandler
{
    [Header("Entity Stats")]
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxHealth = 100f;
    //TODO might need to change from private to public
    [SerializeField] protected bool takeNoDamage = false;
    [SerializeField] private float normalValue = 5f;
    [SerializeField] private float skillValue = 10;
    [SerializeField] private float ultimateValue = 20;
    [SerializeField] private assTeam EntityTeam = assTeam.Unassigned;

    [Header("Entity States")]
    public assEntityState[] EntityStates;
    public assEntityState FirstEntityState;

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

    [Header("Damage Taken")]
    public bool DamageHit = false;
    public float DamageTime;
    public float DamageTimeTrigger = 2f;
    public float DamageAmount;
    public float DamageAmountTrigger = 40f;

    [Header("AI Components")]
    public asAIType AIType = asAIType.Default;
    public float ThinkingTime = 1f;
    public float ShortOffsetThinkResetTime = 1f;
    public float MediumOffsetThinkReset = 2f;
    public float LongOffsetThinkResetTime = 3f;

    //use this only for editor stuff
    [Header("Debug")]
    public Vector3 Velocity;
    public string CurrentState = string.Empty;

    #region Health and Damage Handler
    public float CurrentHP => currentHealth;
    public float MaxHP => maxHealth;
    public bool IsAlive => currentHealth > 0;
    public assTeam Team => EntityTeam;
    public float NormalDamage => normalValue;
    public float SkillDamage => skillValue;
    public float UltimateDamage => ultimateValue;
    public bool IsInvulnerable => takeNoDamage;
    #endregion

    // Start is called before the first frame update
    protected virtual void Start()
    {
        InitializeFSM();
    }

    private void InitializeFSM()
    {
        if (RgdBdy2D == null || AnimCtrl == null) {
            Debug.Break();
            throw new Exception(string.Format("Rigidbody2D or Animator Component unassigned"));
        }

        foreach (assEntityState s in EntityStates) {
            string strType = $"ass{s}State";
            Type t = Type.GetType(strType);
            if (t == null)
                throw new Exception(strType + " doesn't implemented yet");
            IState state = (IState)Activator.CreateInstance(t, this, 0);
            this.RegisterState(state);
        }
        string csString = $"ass{FirstEntityState}State";
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

        //check if touching ground
        Vector3 currentPos = RgdBdy2D.transform.position;
        OnGround = Physics2D.Raycast(currentPos + ColliderOffset,
            Vector2.down, GroundLength, assData.GroundLayer);

        if (DamageHit) {
            DamageTime += Time.deltaTime;
            if (DamageTime >= DamageTimeTrigger) {
                DamageHit = false;
                DamageTime = 0;
                DamageAmount = 0;
            }
        }

        //for debugging to states only
        CurrentState = _existing.ToString();
    }

    private void FixedUpdate()
    {
        if (JumpTimer > Time.time && OnGround) {
            SendMessageToBrain(assMessageType.Jump);
        }
        assPhysicsData.JumpGravityCalculation(RgdBdy2D, FallMultiplier, LowJumpMultiplier);
    }

    //for unity editor purposes only
    protected virtual void OnDrawGizmos()
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

    private void DamageTakenCondition()
    {
        if (!DamageHit) {
            DamageHit = true;
        }
    }

    public virtual void Apply(assHealthDamageType type, assIHealthDamageHandler handler)
    {
        if (IsInvulnerable) {
            Debug.Log("No damage taken to this Entity " + this.gameObject.name);
            return;
        }

        float damageTaken = 0;
        switch (type) {
            case assHealthDamageType.NormalDamage:
                currentHealth = (float)(currentHealth - handler.NormalDamage);
                damageTaken = handler.NormalDamage;
                break;
            case assHealthDamageType.SkillDamage:
                currentHealth = (float)(currentHealth - handler.SkillDamage);
                damageTaken = handler.SkillDamage;
                break;
            case assHealthDamageType.UltimateDamage:
                currentHealth = (float)(currentHealth - handler.UltimateDamage);
                damageTaken = handler.UltimateDamage;
                break;
            case assHealthDamageType.Heal:
                currentHealth = (float)(currentHealth + handler.SkillDamage);
                break;
            case assHealthDamageType.Killed:
                //TODO died
                break;
        }

        if (damageTaken != 0) {
            DamageTakenCondition();
        }

        if (DamageHit) {
            DamageAmount += damageTaken;
            if (DamageAmount >= DamageAmountTrigger) {
                //TODO different entity can have or not have different states
                //testing first state first
                SendMessageToBrain(assMessageType.FirstPhaseActivate);
            }
        }
    }

    public virtual void OnApplyCallback(assHealthDamageType type, assIHealthDamageHandler recipient, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void AIThinkingNextState(asAIType type, float timeToThink)
    {
        switch (type) {
            case asAIType.Default:
                break;
            case asAIType.SmallMinion:
                break;
            case asAIType.AmerAI:
                Debug.Log("thinking?");
                ThinkingTime = timeToThink;
                ChangeState(typeof(assThinkingState));
                break;
            default:
                break;
        }

    }
}

public interface assIHealthDamageHandler
{
    float CurrentHP { get; }
    float MaxHP { get; }
    bool IsAlive { get; }
    bool IsInvulnerable { get; }
    float NormalDamage { get; }
    float SkillDamage { get; }
    float UltimateDamage { get; }
    assTeam Team { get; }
    void Apply(assHealthDamageType type, assIHealthDamageHandler handler);
    void OnApplyCallback(assHealthDamageType type, assIHealthDamageHandler recipient, params object[] args);
}

public enum assHealthDamageType
{
    NormalDamage,
    SkillDamage,
    UltimateDamage,
    Heal,
    Killed,
}