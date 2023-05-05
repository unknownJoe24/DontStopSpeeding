using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{

    [SerializeField] private int carCount;
    [SerializeField] private GameObject[] carPrefab;
    
    

    // Start is called before the first frame update
    void Start()
    {
        carCount = transform.childCount; 
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount < carCount)
        {
            SpawnCar();
        }
    }

    void SpawnCar()
    {
        int randIndex = Random.Range(0, carPrefab.Length);

        var newCar = Instantiate(carPrefab[randIndex], new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z), Quaternion.Euler(0, 90, 0));
        newCar.transform.parent = gameObject.transform; 
    }
}
