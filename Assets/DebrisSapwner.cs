using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSapwner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] Transform minPosition;
    [SerializeField] Transform maxPosition;

    private float randomTimeOffset; 


    // Start is called before the first frame update
    void Start()
    {
        randomTimeOffset = Random.Range(0.25f, 0.75f);
        InvokeRepeating("SpawnObject", 1.0f, 1.0f+randomTimeOffset);
    }

    void SpawnObject()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minPosition.transform.position.x, maxPosition.transform.position.x),
                                             Random.Range(minPosition.transform.position.y, maxPosition.transform.position.y),
                                             Random.Range(minPosition.transform.position.z, maxPosition.transform.position.z));
        Instantiate(objectToSpawn, randomPosition, Random.rotation);
    }
}
