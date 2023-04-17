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

    public GameObject[] transTo2;
    public GameObject[] transTo3;

    // used for lane amount generation
    private int numTwoLanes, numThreeLanes, numRoads;

    // constant indexes to improve readability
    const int easy = 0, diff = 1, hard = 2, safe = 3, trans = 4;

    // what are the roads that are already placed in the scene
    [SerializeField]
    private GameObject[] startRoads;

    // array to hold the last prefabs spawned
    private GameObject[] prev;

    // array to hold the roads to delete
    private GameObject[] del;

    // what is the minimum amount of safe roads between each obstacle
    private int minSafe;

    // when did the game start
    private float startTime;

    // variables for randomness
    private int minVal, maxVal;

    // variables to hold the arrays of the road templates - one is currently empty
    GameObject[][] one;
    GameObject[][] two;
    GameObject[][] three;

    // holds the arrays above
    GameObject[][][] lanes;

    private void Start()
    {
        // initialize arrays two and three
        two = new GameObject[][] { easy2, diff2, hard2, safe2, transTo3 };
        three = new GameObject[][] { easy3, diff3, hard3, safe3, transTo2 };

        // initialize twoLanes and threeLanes
        // holds the amount of obstacles with each number of lanes
        numTwoLanes = numThreeLanes = 0;
        for(int i = 0; i < two.Length - 2; ++i)
        {
            numTwoLanes += two[i].Length;
            numThreeLanes += three[i].Length;
        }

        // numRoads is all roads regardless of number of lanes
        numRoads = numTwoLanes + numThreeLanes;

        // initialize lanes container, one is a dummy container
        lanes = new GameObject[][][] { one , two, three };

        // initialize containers 
        prev = new GameObject[startRoads.Length];
        del = new GameObject[startRoads.Length];

        // initialize prev with the strting roads
        for(int i = 0; i < prev.Length; ++i)
        {
            int startIndex = startRoads.Length - 1 - i;
            if(startIndex >= 0)
                prev[i] = startRoads[startIndex];
        }

        // intialize the minimum amount of safe roads to be spawned between obstacles
        minSafe = 1;

        // initialize chances for what difficulty of road we can spawn
        minVal = 0;
        maxVal = 100;
    }

    private void Update()
    {
        // time since the game has started
        float timeSince = Time.time - startTime;

        // calculates the minimum amount of safe roads between obstacles corresponding to timeSince
        int calcMinSafe = 1 + (int)Mathf.Floor(timeSince / 120f);
        minSafe = calcMinSafe > minSafe ? calcMinSafe : minSafe;

        // adjusts the difficulty
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

    // function to determine what road to spawn, if it needs to be 'safe' i.e. from safe or trans containers
    private GameObject getSafeSpawn(int lanesFrom)
    {
        // new road must begin with three lanes
        if (lanesFrom == 3)
        {
            // initialize curr and changeLanes
            GameObject[] curr;
            float changeLanes = Random.Range(0f, 1f);

            // do we want to transition to two lanes?
            if (changeLanes < ((float)numTwoLanes / (float)numRoads))
                curr = lanes[lanesFrom - 1][trans];
            else
                curr = lanes[lanesFrom - 1][safe];

            // return a random road from the determined container
            int indx = Random.Range(0, curr.Length);
            return (curr[indx]);
        }
        // new road must begin with two lanes
        else
        {
            // initialize curr and changeLanes
            GameObject[] curr;
            float changeLanes = Random.Range(0f, 1f);
            
            // do we want to transition to three lanes?
            if (changeLanes < ((float)numThreeLanes / (float)numRoads))
                curr = lanes[lanesFrom - 1][trans];
            else
                curr = lanes[lanesFrom - 1][safe];

            // return a random road from the determined container
            int indx = Random.Range(0, curr.Length);
            return (curr[indx]);
        }
    }

    // function to determine what difficulty of obstacle to spawn
    private GameObject getObstacleSpawn(int lanesFrom, int prob)
    {
        GameObject[] curr;
        
        // easy obstacle
        if (prob < 100 && (curr = lanes[lanesFrom - 1][easy]).Length > 0)
        {
            curr = lanes[lanesFrom - 1][easy];
            int indx = Random.Range(0, curr.Length);
            return (curr[indx]);
        }
        // difficult obstacle
        else if (prob < 200)
        {
            curr = lanes[lanesFrom - 1][diff];
            int indx = Random.Range(0, curr.Length);
            return (curr[indx]);
        }
        // hard obstacle
        else
        {
            curr = lanes[lanesFrom - 1][hard];
            int indx = Random.Range(0, curr.Length);
            return (curr[indx]);
        }
    }

    // generate a new road
    public void generate()
    {
        // the road to spawn in
        GameObject toSpawn;

        // is a safe road required
        bool obstacle = true;

        // how many safe roads are subsequently in prev
        int numSafe = 0;
        int counter = 0;
        while(counter < prev.Length && !getRoad(prev[counter]).getObstacle())
        {
            ++numSafe;
            ++counter;
        }

        // set obstacle to false if we do not meet the minimum amount of safe roads
        if (numSafe < minSafe)
            obstacle = false;

        // get the probabilites for spawning transitions and obstacles, MAY NEED ADJUSTMENT
        int safeProb = Random.Range(numSafe, minSafe + 5);
        int obstProb = Random.Range(minVal, maxVal);

        // what was the ending amount of lanes for the last road spawned
        int prevLanes = getRoad(prev[0]).getLanes()[1];

        // choose the next thing to spawn
        if (obstacle)
        {
            // there can be more than the required transitions between obstacles if probability dicates, MAY NEED ADJUSTMENT
            if (safeProb < minSafe + 3)
            {
                toSpawn = getSafeSpawn(prevLanes);
            }
            // we want to spawn an obstacle
            else
            {
                toSpawn = getObstacleSpawn(prevLanes, obstProb);
            }
        }
        // we NEED to spawn a number of transitions between every obstacle
        else
        {
            toSpawn = getSafeSpawn(prevLanes);
        }

        // spawn the road and get the instance spawned
        float spawnLength = (getRoad(toSpawn).getLength()) / 2 + (getRoad(prev[0]).getLength() / 2);
        GameObject instance = Instantiate(toSpawn, prev[0].transform.position + new Vector3(0f, 0f, spawnLength), toSpawn.transform.rotation);

        //delete the last road in delete if it exists
        if (del[del.Length - 1] != null)
        {
            Destroy(del[del.Length - 1]);
        }

        // push the elements of delete - mimics queue
        for(int i = del.Length - 1; i >= 0; --i)
        {
            if (i == 0)
                del[i] = prev[prev.Length - 1];
            else
                del[i] = del[i - 1];
        }

        // push the elements of prev - mimics queue
        for(int i = prev.Length - 1; i >= 0; --i)
        {
            if (i == 0)
                prev[i] = instance;
            else
                prev[i] = prev[i - 1];
        }
    }
}
