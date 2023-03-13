using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Road", menuName = "Road")]
public class Road : ScriptableObject
{
    // Is this road an obstacle
    [SerializeField]
    private bool isObstacle;

    // How many lanes does this road have
    [SerializeField]
    private int begLanes;
    [SerializeField]
    private int endLanes;

    // How long is this road
    [SerializeField]
    public float length;

    public bool getObstacle()
    {
        return isObstacle;
    }

    public float getLength()
    {
        return length;
    }

    public int[] getLanes()
    {
        int[] lanes = new int[2];

        lanes[0] = begLanes;
        lanes[1] = endLanes;

        return lanes;
    }
}
