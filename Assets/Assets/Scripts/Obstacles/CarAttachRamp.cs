using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAttachRamp : MonoBehaviour
{


    private Rigidbody _playerRB;
    private GameObject _player; 

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRB = _player.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && RampController.carAttach == false)
        {
            
            RampController.carAttach = true;
            //_playerRB.drag = 1; 
        }

        else if(other.tag == "Player" && RampController.carAttach == true)
        {
            RampController.carAttach = false; 
        }
    }

}
