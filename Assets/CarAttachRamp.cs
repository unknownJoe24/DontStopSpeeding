using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAttachRamp : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && RampController.carAttach == false)
        {
            RampController.carAttach = true; 
        }

        else if(other.tag == "Player" && RampController.carAttach == true)
        {
            RampController.carAttach = false; 
        }
    }

}
