using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class assAIController : assBaseEntity
{
    [Header("Entity AI Conditions")]
    public float HealthPercentChangePhase = 20f;
    public Transform[] TransformationsSpawner;
    public Transform[] RangeSpawnLocations;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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