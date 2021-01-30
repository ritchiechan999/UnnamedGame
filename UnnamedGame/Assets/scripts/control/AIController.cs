using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : BaseEntity
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

        DetectEntity();
    }

    private void DetectEntity()
    {
        Collider2D[] colArray = Physics2D.OverlapCircleAll(RgdBdy2D.transform.position, DetectRadius, Data.EntityLayer);
        if (colArray.Length == 1) {
            Target = null;
            return;
        }

        foreach (Collider2D col in colArray) {
            if (col.gameObject == RgdBdy2D.gameObject)
                continue;

            Target = col.transform;
            SendMessageToBrain(ugMessageType.Move, Target);
        }
    }
}