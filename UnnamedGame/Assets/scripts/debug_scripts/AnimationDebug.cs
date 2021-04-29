using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDebug : MonoBehaviour
{
    public GameObject animateTest;
    public string triggerOne, triggerTwo;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 25) ,triggerOne)) {
            Animator anim = animateTest.GetComponent<Animator>();
            anim.SetTrigger(triggerOne);
            //anim.ResetTrigger(triggerOne);
        }

        if (GUI.Button(new Rect(10, 50, 100, 25), triggerTwo)) {

            Animator anim = animateTest.GetComponent<Animator>();
            anim.SetTrigger(triggerTwo);
            //anim.ResetTrigger(triggerTwo);
        }
    }
}
