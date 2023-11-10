using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairMovement : MonoBehaviour
{

    [SerializeField]
    GameObject joystick;
    [SerializeField]
    GameObject self;

    private Vector3 previousPosition = new Vector3(0,0,0);
    void Update()
    {
        if (previousPosition.x != joystick.transform.localPosition.x || previousPosition.y != joystick.transform.localPosition.y)
        {
            previousPosition = joystick.transform.localPosition;
            self.transform.localPosition = new Vector3(previousPosition.x*6.5f, previousPosition.y*3.5f, previousPosition.z);
        }
    }

}
