using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addForce : MonoBehaviour
{

    [SerializeField] float force;


    // Start is called before the first frame update
    void Start()
    {
        //force = 5;
        GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
    }

}
