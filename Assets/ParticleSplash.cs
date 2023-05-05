using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSplash : MonoBehaviour
{

    public GameObject particleSym;
    public AudioClip splashSound; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SoundManager.Instance.Play(splashSound, 0.5f);
            particleSym.SetActive(true);
        }
    }
}
