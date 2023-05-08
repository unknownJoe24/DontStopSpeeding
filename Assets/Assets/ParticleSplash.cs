using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSplash : MonoBehaviour
{

    public GameObject particleSym;
    public AudioSource particleSound; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            particleSound.Play();
            particleSym.SetActive(true);
        }
    }
}
