using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObstaclePropSpawner : MonoBehaviour
{

    private int objPos;
    public GameObject[] objGroups;

    // Start is called before the first frame update
    void Start()
    {
        objPos = (Random.Range(0, objGroups.Length - 1));
        //print(objPos);
        objGroups[objPos].SetActive(true);
    }
}
