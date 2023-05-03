using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosion : MonoBehaviour
{

    public float radius = 5.0f;
    public float power = 10.0f;
    //public bool test; 

    // Start is called before the first frame update
    void Start()
    {
        Explosion();
    }

    private void Explosion()
    {
        foreach (Transform child in transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if(rb != null)
                rb.AddExplosionForce(power, child.position, radius, 3.0f);
        }
    }
}
