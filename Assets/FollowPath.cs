using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class FollowPath : MonoBehaviour
{

    public PathCreator pathCreator;
    public float speed = 5f;

    float distanceTravel;

    private void Update()
    {
        distanceTravel += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravel);
    }

}
