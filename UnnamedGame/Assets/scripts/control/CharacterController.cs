using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : BaseEntity
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        MovementControl();
    }

    private void MovementControl()
    {
        float hAxis = Input.GetAxis("Horizontal");
        if (hAxis != 0)
            SendMessageToBrain(ugMessageType.Move, hAxis);

        if (Input.GetKeyDown(KeyCode.Space)) {
            JumpTimer = Time.time + JumpDelay;
        }
    }
}
