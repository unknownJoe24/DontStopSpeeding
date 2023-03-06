using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowAnimations : MonoBehaviour
{
    


    private Animator _animtor;
    private AudioSource _audioS; 
    private float randomWalkStart; 


    // Start is called before the first frame update
    void Start()
    {
        randomWalkStart = Random.Range(0.0f, 1.0f);
        _animtor = GetComponent<Animator>();
        _animtor.SetBool("Walking", true);
        _animtor.SetFloat("Offset", randomWalkStart);
        _audioS = GetComponent<AudioSource>();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            _animtor.enabled = false;
            _audioS.Play();
        }
    }
}
