using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLaneMovement : MonoBehaviour
{

    private Rigidbody _rigidbody;
    public float _speed; 

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rigidbody.velocity = transform.forward * _speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Car Despawner")
        {
            Destroy(gameObject);
        }
    }
}