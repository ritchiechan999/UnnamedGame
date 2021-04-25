using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class assCameraController : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Target.transform.position + Offset;
    }
}
