using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrashTruckWithSpawner : MonoBehaviour
{

    private Rigidbody _rigidbody;
    private bool collided; 
    [SerializeField] Transform spawner;
    [SerializeField] GameObject[] trashPrefab; 

    public float _speed; 

    // Start is called before the first frame update
    void Start()
    {
        collided = false;
        _rigidbody = GetComponent<Rigidbody>();
        InvokeRepeating("SpawnObject", 0.75f, 2.0f);
        //_speed = 5.0f; 
    }

    void SpawnObject()
    {
        int randIndex = Random.Range(0, trashPrefab.Length);
        if (!collided)
        {

            for (int i = 0; i < 5; i++)
            {
                Instantiate(trashPrefab[randIndex], spawner.position, Random.rotation);
                randIndex = Random.Range(0, trashPrefab.Length);
            }
            
        }
        else
        {
            return; 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ExplosionObjects")
        {
            collided = true;
            print("Explosion!");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!collided)
        {
            _rigidbody.velocity = -transform.forward * _speed * Time.deltaTime;
        }
        
    }
}
