using System.Collections;
using UnityEngine;


public class Debugger : MonoBehaviour, assIHealthDamageHandler
{
    public GameObject DebugObject;
    public float testDamage = 20f;

    assAIController ai;

    #region Health Handler
    public float CurrentHP => throw new System.NotImplementedException();
    public float MaxHP => throw new System.NotImplementedException();
    public bool IsAlive => throw new System.NotImplementedException();
    public bool IsInvulnerable => throw new System.NotImplementedException();
    public float NormalDamage => testDamage;
    public float SkillDamage => throw new System.NotImplementedException();
    public float UltimateDamage => throw new System.NotImplementedException();
    public assTeam Team => throw new System.NotImplementedException(); 
    #endregion

    // Use this for initialization
    void Start()
    {
        ai =  DebugObject.GetComponent<assAIController>();


    }

    // Update is called once per frame
    void Update()
    {
        DebugFunction();
    }

    private void DebugFunction()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ai.Apply(assHealthDamageType.NormalDamage, this);
        }
    }

    public void Apply(assHealthDamageType type, assIHealthDamageHandler handler)
    {
        throw new System.NotImplementedException();
    }

    public void OnApplyCallback(assHealthDamageType type, assIHealthDamageHandler recipient, params object[] args)
    {
        throw new System.NotImplementedException();
    }
}
