using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTrash : MonoBehaviour
{
    [SerializeField] float delay;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            Destroy(this.gameObject, delay);
        }
    }
}
