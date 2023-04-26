using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class FollowPath : MonoBehaviour
{
    private GameObject pathObj; 
    private PathCreator pathCreator;
    public float speed = 5f;

    float distanceTravel;

    private void Start()
    {
        pathObj = GameObject.Find("Path");
        pathCreator = pathObj.GetComponent<PathCreator>();
    }

    private void Update()
    {
        distanceTravel += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravel);
    }

}
