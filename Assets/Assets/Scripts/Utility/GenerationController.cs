using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationController : MonoBehaviour
{
    // What is the generator
    [SerializeField]
    private GameObject generatorObject;

    private Generation generatorScript;

    // Start is called before the first frame update
    void Start()
    {
        generatorScript = generatorObject.GetComponent<Generation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Spawner"))
        {
            other.gameObject.SetActive(false);

            generatorScript.generate();
        }
    }
}
