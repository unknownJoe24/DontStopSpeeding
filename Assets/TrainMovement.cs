using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{

    private Rigidbody _rigidbody;
    public float _speed;

    static public bool _trainTrigger; 

    // Start is called before the first frame update
    void Start()
    {
        _trainTrigger = false; 
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_trainTrigger)
        {
            _rigidbody.velocity = new Vector3(-1, 0, 0) * _speed * Time.deltaTime;
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Despawner")
        {
            Destroy(gameObject);
        }
    }
}
