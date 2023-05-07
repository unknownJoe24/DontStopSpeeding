using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampController : MonoBehaviour
{

    private LaneSwitcher playerInfo;                    // Reference to the playerInfo speed (might use as basis for deciding speed for moving in LerpToPoints();

    private bool _rampedUp;

    private void Awake()
    {
        _rampedUp = LaneSwitcher.rampedUp;
        
    }

    private void Update()
    {
        _rampedUp = LaneSwitcher.rampedUp;

        if(_rampedUp)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }

}
