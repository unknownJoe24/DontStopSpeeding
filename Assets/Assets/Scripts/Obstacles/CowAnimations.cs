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

    // this is here because the object holding CowCrossing_WayPoints does not have a collider
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // renable all rotation
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            // disable animation - note bounciness stil applies
            _animtor.enabled = false;
            // no longer move
            gameObject.transform.parent.gameObject.GetComponent<CowCrossing_WayPoints>().setMove(false);

            _audioS.Play();
            Debug.Log("Cow is being disabled.");
        }
    }
}
