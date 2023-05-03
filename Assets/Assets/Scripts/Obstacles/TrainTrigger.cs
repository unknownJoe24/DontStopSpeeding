using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTrigger : MonoBehaviour
{

    public AudioClip trainClip;
    public float volume;

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            SoundManager.Instance.Play(trainClip, volume);
            print("Train Moving");
            TrainMovement._trainTrigger = true; 
        }

    }

}
