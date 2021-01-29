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

        Vector3 currentPos = RgdBdy2D.transform.position;
        OnGround = Physics2D.Raycast(currentPos + ColliderOffset,
                     Vector2.down, GroundLength, Data.GroundLayer);

        if (Input.GetKeyDown(KeyCode.Space)) {
            JumpTimer = Time.time + JumpDelay;
        }
    }
}
