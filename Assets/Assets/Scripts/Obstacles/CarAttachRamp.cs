using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAttachRamp : MonoBehaviour
{

    public static bool carAttach;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && carAttach == false)
        {
            print("Car has contacted ramp");
            carAttach = true;
        }
    }

}
