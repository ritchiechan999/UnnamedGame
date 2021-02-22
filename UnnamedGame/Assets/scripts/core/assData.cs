using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class assData
{
    private const string groundLayerName = "ground";
    private const string entityLayerName = "entity";
    private const string interactableLayerName = "interactable";
    public static int GroundLayer => 1 << LayerMask.NameToLayer(groundLayerName);
    public static int EntityLayer => 1 << LayerMask.NameToLayer(entityLayerName);
    public static int InteractableLayer => 1 << LayerMask.NameToLayer(interactableLayerName);
}

public static class assPhysicsData
{
    public const float GlobalGravity = -9.8f;
    private static float gravityScale;

    private static float verticalGravity = Physics2D.gravity.y;

    //for 3D
    public static void ModifyPhysics(bool onGround, Rigidbody rb, float customGravity,
                                        float dragScale, float fallMultiplier, float horizontalMovement)
    {
        if (onGround) {
            rb.drag = Mathf.Abs(horizontalMovement) < 0.4f ? dragScale : 0;
            gravityScale = 0;
        } else {
            gravityScale = customGravity;
            rb.drag = dragScale * 0.15f;
            float rbVelocityY = rb.velocity.y;
            Vector3 newGravity = GlobalGravity * gravityScale * Vector3.up;
            if (rbVelocityY < 0) {
                newGravity *= fallMultiplier;
            } else if (rbVelocityY > 0 && !Input.GetButton("Jump")) {
                newGravity *= fallMultiplier / 2;
            }
            rb.AddForce(newGravity, ForceMode.Acceleration);
        }
    }

    public static void JumpGravityCalculation(Rigidbody2D rb, float fallMultiplier, float lowFallMultiplier)
    {
        if (rb.velocity.y < 0) {
            rb.velocity += Vector2.up * verticalGravity * (fallMultiplier - 1) * Time.deltaTime;
        }else if(rb.velocity.y > 0 && !Input.GetButton("Jump")) {
            rb.velocity += Vector2.up * verticalGravity * (lowFallMultiplier - 1) * Time.deltaTime;
        }
    }
}

public enum assTeam
{
    Unassigned,
    Team1,
    Team2,

}

//NOTE: will be using the new combo
/*
//combo handler stuffs
public enum tdAttackAnimState {
    Unassigned,
    //main combos 1-99
    Light = 1,
    Heavy = 2,
    Magic = 3,

    //fin combo 100-199
    TwoInputCombo = 100,
    ThreeInputCombo,
    FourInputCombo,
}

public enum tdAttackType {
    Heavy = 0,
    Light = 1,
    Magic = 2,
}

[Serializable]
public class tdAttack {
    public string Name;
    //TODO auto fill up length duration
    public float LengthDuration;
    public tdAttackAnimState AnimState;
}

[Serializable]
public class tdComboInput {
    public tdAttackType Type;
    public tdComboInput(tdAttackType t) {
        Type = t;
    }
    public bool IsSameAs(tdComboInput comboInput) {
        return (Type == comboInput.Type);
    }
}

[Serializable]
public class tdCombo {
    public string Name;
    public List<tdComboInput> Inputs;
    public tdAttack ComboAttack;
    public Action OnInputted;
    int _curInputIdx = 0;
    [HideInInspector]
    public bool DoComboAtk;

    public bool ContinueCombo(tdComboInput input) {
        if (Inputs[_curInputIdx].IsSameAs(input)) {
            _curInputIdx++;
            //Debug.Log(_curInputIdx);
            if (_curInputIdx >= Inputs.Count) {
                DoComboAtk = true;
                OnInputted.Invoke();
                _curInputIdx = 0;
            }
            return true;
        } else {
            _curInputIdx = 0;
            return false;
        }
    }

    public tdComboInput CurrentComboInput() {
        if (_curInputIdx >= Inputs.Count)
            return null;
        return Inputs[_curInputIdx];
    }

    public void ResetCombo() {
        _curInputIdx = 0;
        DoComboAtk = false;
    }
}
*/