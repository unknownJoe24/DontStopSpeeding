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

    // dictionary for ease of use
    const int easy = 0, diff = 1, hard = 2, safe = 3, trans = 4;

    // what is the first road placed
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

    // variables to hold the arrays of the road templates
    GameObject[][] one;
    GameObject[][] two;
    GameObject[][] three;

    GameObject[][][] lanes;

    private void Start()
    {
        // initialize dictionary
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
        numRoads = numTwoLanes + numThreeLanes;

        // initialize lanes container, one is a dummy container
        lanes = new GameObject[][][] { one , two, three };

        // initialize containers 
        prev = new GameObject[3];
        del = new GameObject[startRoads.Length];

        // initialize array
        for(int i = 0; i < prev.Length; ++i)
        {
            int startIndex = startRoads.Length - 1 - i;
            if(startIndex >= 0)
                prev[i] = startRoads[startIndex];
        }

        minSafe = 1;

        // initialize chances for what difficulty of road we can spawn
        minVal = 0;
        maxVal = 100;
    }

    private void Update()
    {
        float timeSince = Time.time - startTime;

        int calcMinSafe = 1 + (int)Mathf.Floor(timeSince / 120f);
        minSafe = calcMinSafe > minSafe ? calcMinSafe : minSafe;

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

    // function to determine what road to spawn if it needs to be 'safe' i.e. from safe or trans containers
    private GameObject getSafeSpawn(int lanesFrom)
    {
        if (lanesFrom == 3)
        {
            GameObject[] curr;
            float changeLanes = Random.Range(0f, 1f);

            // we want to transition to two lanes
            if (changeLanes < ((float)numTwoLanes / (float)numRoads))
                curr = lanes[lanesFrom - 1][trans];
            else
                curr = lanes[lanesFrom - 1][safe];

            int indx = Random.Range(0, curr.Length);
            return (curr[indx]);
        }
        else
        {
            GameObject[] curr;
            float changeLanes = Random.Range(0f, 1f);

            if (changeLanes < ((float)numThreeLanes / (float)numRoads))
                curr = lanes[lanesFrom - 1][trans];
            else
                curr = lanes[lanesFrom - 1][safe];

            int indx = Random.Range(0, curr.Length);
            return (curr[indx]);
        }
    }

    // function to determine what obstacle to spawn
    private GameObject getObstacleSpawn(int lanesFrom, int prob)
    {
        GameObject[] curr;
        if (prob < 100 && (curr = lanes[lanesFrom - 1][easy]).Length > 0)
        {
            curr = lanes[lanesFrom - 1][easy];
            int indx = Random.Range(0, curr.Length);
            return (curr[indx]);
        }
        else if (prob < 200)
        {
            curr = lanes[lanesFrom - 1][diff];
            int indx = Random.Range(0, curr.Length);
            return (curr[indx]);
        }
        else
        {curr = lanes[lanesFrom - 1][hard];
            int indx = Random.Range(0, curr.Length);
            return (curr[indx]);
        }
    }

    // generate a new piece
    public void generate()
    {
        GameObject toSpawn;

        // is a safe road required
        bool obstacle = true;
/*        if (prev[0].GetComponent<RoadController>().myInfo.getObstacle())
            obstacle = false;
*/
        // how many safe roads are in prev
        int numSafe = 0;
        if (obstacle)
        {
            int counter = 0;
            while(counter < prev.Length && !getRoad(prev[counter]).getObstacle())
            {
                ++numSafe;
                ++counter;
            }
        }

        Debug.Log("minSafe: " + minSafe + "\nnumSafe: " + numSafe);
        if (numSafe < minSafe)
        {
            Debug.Log("Disabling Obstacles");
            obstacle = false;
        }

        // get the probabilites for spawning transitions and obstacles
        int safeProb = Random.Range(numSafe, 5 * minSafe);      // MAY NEED ADJUSTED
        int obstProb = Random.Range(minVal, maxVal);

        // what was the ending amount of lanes for the last road spawned
        int prevLanes = getRoad(prev[0]).getLanes()[1];

        // choose the next thing to spawn
        if (obstacle)
        {
            // there can be more than one transition between obstacles if probability dicates
            if (safeProb < 3)
            {
                toSpawn = getSafeSpawn(prevLanes);
            }
            // we want to spawn an obstacle
            else
            {
                toSpawn = getObstacleSpawn(prevLanes, obstProb);
            }
        }
        // we NEED to spawn a transition between every obstacle
        else
        {
            toSpawn = getSafeSpawn(prevLanes);
        }

        Debug.Log("MIN: " + minVal + "\n MAX: " + maxVal);
        Debug.Log(obstProb + "lead to " + toSpawn + "being selected");

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
