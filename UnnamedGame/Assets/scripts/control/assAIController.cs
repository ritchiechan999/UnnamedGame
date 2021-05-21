using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class assAIController : assBaseEntity
{
    [Header("Entity AI Conditions")]
    public float HealthPercentChangePhase = 20f;
    public Transform[] TransformationsSpawner;
    public Transform[] RangeSpawnLocations;

    [Header("Debug")]
    public string TargetName = "Shop Location";

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Initialize();
    }

    private void Initialize()
    {
        switch (AIType) {
            case asAIType.Default:
                IsAIBoss = false;
                break;
            case asAIType.SmallMinion:
                IsAIBoss = false;
                break;
            case asAIType.AmerAI:
                IsAIBoss = true;
                //debug only, to be removed or get the target from the game manager
                Target = GameObject.Find(TargetName).transform;
                break;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!IsAIBoss)
            DetectEntity();
    }

    private void DetectEntity()
    {
        Collider2D[] colArray = Physics2D.OverlapCircleAll(RgdBdy2D.transform.position, DetectRadius, assData.EntityLayer);
        if (colArray.Length == 1) {
            Target = null;
            return;
        }

        foreach (Collider2D col in colArray) {
            if (col.gameObject == RgdBdy2D.gameObject)
                continue;

            Target = col.transform;
            SendMessageToBrain(assMessageType.Move, Target);
        }
    }

    public override void Apply(assHealthDamageType type, assIHealthDamageHandler handler)
    {
        Debug.Log("applying damage");
        base.Apply(type, handler);
        CurrentHealthStatus();
    }

    public override void OnApplyCallback(assHealthDamageType type, assIHealthDamageHandler recipient, params object[] args)
    {
        base.OnApplyCallback(type, recipient, args);
    }

    private void CurrentHealthStatus()
    {
        float changePhasePercent = HealthPercentChangePhase / MaxHP;
        float healthChangePhase = MaxHP * changePhasePercent;
        if (CurrentHP <= healthChangePhase) {
            //TODO change phase
            takeNoDamage = true;
            SendMessageToBrain(assMessageType.FinalPhaseActivate, IsInvulnerable);
        }
    }
}

/// <summary>
/// classifications of AI. all can be categorize here
/// </summary>
public enum asAIType
{
    Default,
    SmallMinion,
    AmerAI,

}