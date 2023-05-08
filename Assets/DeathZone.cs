using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

    private void Start()
    {
        PlayerHealth healthInfo = gameObject.GetComponent<PlayerHealth>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerHealth healthInfo = FindObjectOfType<PlayerHealth>();
            healthInfo.killPlayer();
        }
    }

}
