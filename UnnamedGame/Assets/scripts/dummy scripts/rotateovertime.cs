using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateovertime : MonoBehaviour
{
    public float RotationSpeed;
    public Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(dir * (RotationSpeed * Time.deltaTime));
    }
}
