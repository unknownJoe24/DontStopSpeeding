using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    // arrays to hold the prefabs to spawn
    public GameObject[] easy2;
    public GameObject[] easy3;
    public GameObject[] diff2;
    public GameObject[] diff3;
    public GameObject[] hard2;
    public GameObject[] hard3;
    public GameObject[] safe2;
    public GameObject[] safe3;

    // dictionary for ease of use
    private Dictionary<int, GameObject[][]> roadDictionary;
    const int easy = 0, diff = 1, hard = 2, safe = 3;

    // what is the first road placed
    [SerializeField]
    private GameObject[] startRoads;

    // How many roads were placed manually
    private int initial;

    // array to hold the last prefabs spawned
    private GameObject[] prev;

    // array to hold the roads to delete
    private GameObject[] del;

    // when did the game start
    private float startTime;

    // variables for randomness
    private int minVal, maxVal;

    // variable to hold the position to spawn the next thing
    private Vector3 nextPos;

    // variables to hold the arrays of the road templates
    GameObject[][] one;
    GameObject[][] two;
    GameObject[][] three;

    GameObject[][][] gross;

    private void Start()
    {
        // initialize dictionary
        two = new GameObject[][] { easy2, diff2, hard2, safe2 };
        three = new GameObject[][] { easy3, diff3, hard3, safe3 };

        //roadDictionary.Add(2, two);
        //roadDictionary.Add(3, three);

        gross = new GameObject[][][] { one , two, three };

        initial = startRoads.Length;

        prev = new GameObject[3];
        del = new GameObject[startRoads.Length];

        // initialize array
        for(int i = 0; i < prev.Length; ++i)
        {
            int startIndex = startRoads.Length - 1 - i;
            if(startIndex >= 0)
                prev[i] = startRoads[startIndex];
        }

        // set the position to spawn the first road
        nextPos = startRoads[0].transform.position;
        for(int i = 1; i < startRoads.Length; ++i)
            nextPos += startRoads[i].transform.position;

        minVal = 0;
        maxVal = 100;
    }

    private void Update()
    {
        float timeSince = Time.time - startTime;

        // difficulty
        if(timeSince > 60 && timeSince <= 120)
        {
            minVal = 50;
            maxVal = 150;
        }
        else if(timeSince > 120)
        {
            minVal = 0;
            maxVal = 300;
        }
    }

    // simple private accessor for simplicity of code
    private Road getRoad(GameObject _target)
    {
        return _target.GetComponent<RoadController>().myInfo;
    }

    // accessor for initial
    public int getInitial()
    {
        return initial;
    }

    // decrements initial roads and deletes them if necessary
    public void decrementInitial()
    {
        // decrement initial and destroy all starting roads - except those that were placed in prev in start
        if(--initial <= -1)
        {
            //for (int i = 0; i < startRoads.Length; ++i)
                //Destroy(startRoads[i]);


        }
    }

    // generate a new piece
    public void generate()
    {
        GameObject toSpawn;

        // is a safe road required
        bool obstacle = true;
        if (prev[0].GetComponent<RoadController>().myInfo.getObstacle())
            obstacle = false;

        // how many safe roads are in prev
        int numSafe = 0;
        if (obstacle)
        {
            for (int i = 0; i < prev.Length; ++i)
            {
                if (prev[i] != null && !prev[i].GetComponent<RoadController>().myInfo.getObstacle())
                {
                    ++numSafe;
                }
            }
        }

        int safeProb = Random.Range(numSafe, 5);
        int obstProb = Random.Range(minVal, maxVal);
        int prevLanes = getRoad(prev[0]).getLanes()[1];

        // choose the next thing to spawn
        if (obstacle)
        {
            // there's required to be a safe for every amount of lanes
            if (safeProb < 3)
            {
                GameObject[] curr = gross[prevLanes-1][safe];
                toSpawn = curr[Random.Range(0, curr.Length)];
            }
            else
            {
                GameObject[] curr;
                if (obstProb < 100 && (curr = gross[prevLanes-1][easy]).Length > 0)
                {
                    curr = gross[prevLanes-1][easy];
                    toSpawn = curr[Random.Range(0, curr.Length)];
                }
                else if (obstProb < 200)
                {
                    curr = gross[prevLanes-1][diff];
                    toSpawn = curr[Random.Range(0, curr.Length)];
                }
                else
                {
                    curr = gross[prevLanes-1][hard];
                    toSpawn = curr[Random.Range(0, curr.Length)];
                }
            }
        }
        else
        {
            GameObject[] curr = gross[prevLanes-1][safe];
            toSpawn = curr[Random.Range(0, curr.Length - 1)];
        }
        Debug.Log("MIN: " + minVal + "\n MAX: " + maxVal);
        Debug.Log(obstProb + "lead to " + toSpawn + "being selected");
        // spawn the road
        //Debug.Log("Before Instantiation: " + toSpawn);
        float spawnLength = (toSpawn.GetComponent<RoadController>().myInfo.getLength()) / 2 + (prev[0].GetComponent<RoadController>().myInfo.getLength() / 2);
        //GameObject instance = Instantiate(toSpawn, nextPos + new Vector3(0f, 0f, spawnLength), Quaternion.identity);
        GameObject instance = Instantiate(toSpawn, prev[0].transform.position + new Vector3(0f, 0f, spawnLength), toSpawn.transform.rotation);

        //nextPos.z += toSpawn.GetComponent<RoadController>().myInfo.length;

        if (del[del.Length - 1] != null)
        {
            Destroy(del[del.Length - 1]);
        }


        //update and move the delete
        for(int i = del.Length - 1; i >= 0; --i)
        {
            if (i == 0)
                del[i] = prev[prev.Length - 1];
            else
                del[i] = del[i - 1];
        }

        // temporary - should probably make a class
        //prev[2] = prev[1];
        //prev[1] = prev[0];
        //prev[0] = instance;
        for(int i = prev.Length - 1; i >= 0; --i)
        {
            if (i == 0)
                prev[i] = instance;
            else
                prev[i] = prev[i - 1];
        }

    }
}
