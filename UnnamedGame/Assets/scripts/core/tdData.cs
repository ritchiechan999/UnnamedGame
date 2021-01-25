using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class tdData {
    const string _groundLayerName = "ground";
    public static int GroundLayer => 1 << LayerMask.NameToLayer(_groundLayerName);
}

public static class tdPhysicsData {
    public const float GlobalGravity = -9.8f;
    static float _gravityScale;

    public static void ModifyPhysics(bool onGround, Rigidbody rb, float customGravity,
                                        float dragScale, float fallMultiplier, float horizontalMovement) {
        if (onGround) {
            rb.drag = Mathf.Abs(horizontalMovement) < 0.4f ? dragScale : 0;
            _gravityScale = 0;
        } else {
            _gravityScale = customGravity;
            rb.drag = dragScale * 0.15f;
            float rbVelocityY = rb.velocity.y;
            Vector3 newGravity = GlobalGravity * _gravityScale * Vector3.up;
            if (rbVelocityY < 0) {
                newGravity *= fallMultiplier;
            } else if (rbVelocityY > 0 && !Input.GetButton("Jump")) {
                newGravity *= fallMultiplier / 2;
            }
            rb.AddForce(newGravity, ForceMode.Acceleration);
        }
    }
}

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

