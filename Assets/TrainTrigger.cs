using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            print("Train Moving");
            TrainMovement._trainTrigger = true; 
        }

    }

}
