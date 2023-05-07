using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTrigger : MonoBehaviour
{

    public float volume;

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            print("Train Moving");
            TrainMovement._trainTrigger = true; 
        }

    }

}
