using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            print("Player has reached point");
            RampController.pointCounter += 1;
            
        }
        else
        {
            print(other.tag);
        }
    }

}
