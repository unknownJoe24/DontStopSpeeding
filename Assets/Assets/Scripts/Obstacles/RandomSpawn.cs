using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour

{
    private int randomLane1;
    List<int> RandomMissingLaneList = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateRandomNumber();
       
        int i = 0; 
        foreach(Transform child in transform)
        {
            if(i == RandomMissingLaneList[0] || i == RandomMissingLaneList[1])
            {
                child.gameObject.SetActive(false);
            }
            i++; 
        }
        
    }

    void GenerateRandomNumber()
    {
        RandomMissingLaneList = new List<int>(new int[2]);
        for(int j = 0; j <= 1; j++)
        {
            randomLane1 = Random.Range(1, transform.childCount);

            while(RandomMissingLaneList.Contains(randomLane1))
            {
                randomLane1 = Random.Range(1, transform.childCount);
            }

            RandomMissingLaneList[j] = randomLane1;

            //print(RandomMissingLaneList[j]);
        }
    }
}
