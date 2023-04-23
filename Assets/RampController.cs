using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampController : MonoBehaviour
{

    private bool _rampedUp;
    public bool testTrigger; 

    private void Awake()
    {
        _rampedUp = LaneSwitcher.rampedUp;
        testTrigger = false; 
    }

    private void Update()
    {

        _rampedUp = LaneSwitcher.rampedUp;

        if(_rampedUp || testTrigger)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        //print(testTrigger);
    }


    private void SpawnRamps()
    {
        if(_rampedUp || testTrigger)
        {
            //print("Ramp Should Spawn");
            transform.GetChild(0).gameObject.SetActive(true);
        }

        //testing
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }

}
