using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour

{
    private int randomLane1, randomLane2; 

    // Start is called before the first frame update
    void Start()
    {

        randomLane1 = Random.Range(1, transform.childCount);
        randomLane2 = Random.Range(1, transform.childCount);
        int i = 1; 
        foreach(Transform child in transform)
        {
            if(i == randomLane1 || i == randomLane2)
            {
                child.gameObject.SetActive(false);
            }
            i++; 
        }
    }

    void GenerateRandomNumber()
    {
        
    }
}
