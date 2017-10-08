using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p2testInputHandler : MonoBehaviour
{

    public bool mouseDown;

    // Use this for initialization
    void Start()
    {
        mouseDown = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void inputUpdate()
    {
        if (UnityEngine.Input.GetMouseButton(1))
        {
            mouseDown = true;
        }
        else
        {
            mouseDown = false;
        }
    }
}