using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSapwner : MonoBehaviour
{
    [SerializeField] GameObject[] DebrisPrefab; 
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] Transform minPosition;
    [SerializeField] Transform maxPosition;
    private int randIndex;

    private float randomTimeOffset; 


    // Start is called before the first frame update
    void Start()
    {
        randIndex = Random.Range(0, DebrisPrefab.Length);
        randomTimeOffset = Random.Range(0.25f, 0.75f);
        InvokeRepeating("SpawnObject", 1.0f, 3.0f+randomTimeOffset);
    }

    void SpawnObject()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minPosition.transform.position.x, maxPosition.transform.position.x),
                                             Random.Range(minPosition.transform.position.y, maxPosition.transform.position.y),
                                             Random.Range(minPosition.transform.position.z, maxPosition.transform.position.z));
        Instantiate(DebrisPrefab[randIndex], randomPosition, Random.rotation);
        randIndex = Random.Range(0, DebrisPrefab.Length);
    }
}
