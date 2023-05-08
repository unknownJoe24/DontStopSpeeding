using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LaneSwitcher : MonoBehaviour
{
    public float moveSpeed = 5f;                            // speed at which player moves forward
    private Rigidbody rb;                                   // rigidbody of the player
    private Vector3 rayPos;                                 // where does the ray that checks beneath the car come from
    public float forwardForce = 10000f;                     // force to move the car forwards
    public float interpolationSpeed = 0.05f;                // determines the speed of the lerp used for speed


    public Transform[] twoLaneTransforms;                   // the transforms for moving in a two lane road
    public Transform[] threeLaneTransforms;                 // the transforms for moving in a three lane road
    private int currentLane = 1;                            // the current lane that the player is in
    private bool isChangingLane = false;                    // is the player changing lanes
    private int targetLane = 1;                             // which lane is the player moving to
    public bool hasThreeLanes = true;                       // does the road the player is "on" have three lanes

    [Header("Audio")]
    public AudioClip speedBoost;

    [Header("State Checks")]
    public bool gearChange = false;                         // did the player change their gear

    [Header("Upgrade-Handling Variables")]
    // Upgrade-Handling Variables
    public bool armored = false;                            // does the player have the armor upgrade

    private bool prevLiquid = false;                        // was the player in liquid the last frame
    public bool amphibious = false;                         // does the player have the amphibious upgrade
    private bool ampActive;                                 // is the amphibious effect currently active
    public float ampTime = 3f;                              // how long does the amphibious effect last
    private float ampStart;                                 // when did the amphibious effect start

    public bool sponsored = false;                          // does the player have the Al upgrade
    private float alTimer;                                  // when did the last ad play
    private float alTime;                                   // how often does the ad play

    [SerializeField]
    private VideoPlayer vPlayer;                            // the video player to play the ad
    
    static public bool rampedUp = false;                    // this is being set to static for Ramp Controller script

    [Header("Car Settings")]
    public float speedIncrement = 10f;                      // how much does the speed increase by
    //public float maxSpeed = 50f;                          // max speed of the player
    public float speedInc;                                  // how often does the player's speed increase
    private float sinceInc;                                 // how long has it been since the player's speed increased

    [Header("Gear Settings")]
    public int currentGear = 1;                             // the current gear the player is in
    public int maxGears = 3;                                // how many gears does the player have

    public float[] baseMinSpeeds = { 50f, 70f, 100f };      // what are the baseline minimum gear speeds
    public float[] baseMaxSpeeds = { 70f, 100f, 120f };     // what are the baseline maximum gear speeds

    [SerializeField]
    private float[] gearMinSpeeds;                          // what are the real time minimum gear speeds
    [SerializeField]
    private float[] gearMaxSpeeds;                          // what are the real time maximum gear speeds


    private float minSpeed;                                 // what is the lowest speed the player can go without dying
    private float sinceStart, startMinSpeed;                // how long has the player been playing
                                                            // and how long until we start checking if the player dropped below the min speed

    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;             // the volume of audio

    [SerializeField] float speed = 0.0f;                    // the speed the player is moving
    public float Speed => speed;                            // used to access and modify speed


    private bool landing = false;

    [SerializeField] bool disableMovement;                  // can the player move

    public bool DisableMovement
    {
        get => disableMovement;
        set => disableMovement = value;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        ampStart = -1f;

        sinceInc = 0f;

        // copy base speeds into real time speeds
        for(int i = 0; i < maxGears; ++i)
        {
            gearMinSpeeds[i] = baseMinSpeeds[i];
            gearMaxSpeeds[i] = baseMaxSpeeds[i];
        }

        // set the initial minimum speed based on the current gear
        minSpeed = baseMinSpeeds[0] - 30f;

        // don't start checking for the min speed right away, start 30 seconds after start for every 100 speed
        sinceStart = 0;
        startMinSpeed = (gearMinSpeeds[0] / 100f) * 30f;

        // set the initial speed to be equal to the initial minimum speed
        speed = gearMinSpeeds[0];

        // set the initial movement velocity of RB
        rb.velocity = new Vector3(0f, 0f, speed);
    }

    void Update()
    {
        // gets the speed of the player?
        speed = transform.InverseTransformDirection(rb.velocity).z;

        // get the starting position for the below-checking raycast
        rayPos = gameObject.transform.position;
        rayPos.z = GameObject.Find("FLWheel").transform.position.z;

        // get input for changing gears
        gearChange = Input.GetButtonDown("Change Gear");

        // Check if the player is moving horizontally and change the target lane accordingly
        // note: getButtonDown
        if (Input.GetAxisRaw("Horizontal") < 0 && currentLane > 0 && !isChangingLane)   // move left
        {
            targetLane = currentLane - 1;
            isChangingLane = true;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0 && currentLane < (hasThreeLanes ? threeLaneTransforms.Length : twoLaneTransforms.Length) - 1 && !isChangingLane)    // move right
        {
            targetLane = currentLane + 1;
            isChangingLane = true;
        }

        // Move the player to the center of the current or target lane
        Transform[] currentTransforms = hasThreeLanes ? threeLaneTransforms : twoLaneTransforms;
        Vector3 targetPosition = currentTransforms[isChangingLane ? targetLane : currentLane].position;
        targetPosition.y = transform.position.y;
        targetPosition.z = transform.position.z;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the player has reached the target lane and stop changing lanes
        if (isChangingLane && Mathf.Approximately(transform.position.x, targetPosition.x))
        {
            currentLane = targetLane;
            isChangingLane = false;
        }

        // Gear change
        if (gearChange)
        {
            ChangeGear();
        }

        // handle Better Call Al's sponsorship segment
        if (sponsored && Time.time - alTimer > alTime)
        {
            // Better Call Al!
            StartCoroutine(vPlayer.GetComponent<StreamVideo>().PlayVideo());
            alTimer = Time.time;
            alTime = 200f + (Random.Range(0f, 1f) * 300f);
        }
        //makes sure the player lands without dying by falling below the minimum speed
        controlLanding();

        // make sure the player does not drop below the minimum speed, also increases base speeds over time
        handleMinSpeed();

        // hangle liquid behavior
        handleLiquids();

        // adjust the speed of the player
        AdjustSpeed();
    }

    private void FixedUpdate()
    {
        // add force to move the player if they can
        if (!disableMovement)
        {
            rb.AddForce(transform.forward * forwardForce);
        }

        // slow the player to a halt if movement is disabled
        if (disableMovement && speed > 0.0f)
        {
            rb.drag += 0.05f;
        }

    }



    // handles the increase in the minimum speed and kills the player if the fall below it
    void handleMinSpeed()
    {
        // increase the max speeds of the player if necessary
        sinceInc += Time.deltaTime;
        if (sinceInc >= speedInc)
        {
            increaseBaseMaxSpeed(10f);
            minSpeed += 10;
            sinceInc = 0f;
        }

        PlayerHealth healthInfo = gameObject.GetComponent<PlayerHealth>();
        BombDefusal bombInfo = GameObject.Find("DefusalHandler").GetComponent<BombDefusal>(); // had ?

        if (healthInfo == null)
        {
            Debug.LogError("PlayerHealth component is missing on the game object.");
            return;
        }

        if (bombInfo == null)
        {
            Debug.LogError("BombDefusal component is missing on the DefusalHandler game object.");
            return;
        }

        if (carFlipped())
        {
            healthInfo.killPlayer();
        }

        // make sure the player is going fast enough if startMinSpeed has elapsed since the start of the game
        sinceStart += Time.deltaTime;

        if (sinceStart >= startMinSpeed && speed < minSpeed && !healthInfo.dead && !bombInfo.getCompleted() && groundCheck(LayerMask.GetMask("Default")) && landing == false)
                                                                                                               //^ this was added in the ToDoListSarah Branch as an attempt to solve the dying upon hitting the ramp 
        {
            healthInfo.killPlayer();
        }

    }

    void AdjustSpeed()
    {
        // Get the max speed for the current gear
        float gearMaxSpeed = gearMaxSpeeds[currentGear - 1];

        // Check if the current speed is greater than the gear's max speed
        if (speed > gearMaxSpeed)
        {
            // Use Mathf.Lerp to smoothly adjust the speed to the gear's max speed
            // The third parameter (0.05f) controls the interpolation speed; adjust this value to change how quickly the car's speed adjusts to the maxSpeed for the current gear
            float newSpeed = Mathf.Lerp(speed, gearMaxSpeed, interpolationSpeed);
            Vector3 newVelocity = rb.velocity;
            newVelocity.z = newSpeed;
            rb.velocity = newVelocity;
        }
    }


    void ChangeGear()
    {
        // Initialize a variable to determine the direction of the gear change
        int gearChangeDirection = 0;

        // Check if the player is trying to upshift
        if (Input.GetAxisRaw("Change Gear") > 0)
        {
            gearChangeDirection = 1;
        }
        // Check if the player is trying to downshift
        else if (Input.GetAxisRaw("Change Gear") < 0)
        {
            gearChangeDirection = -1;
        }

        // Calculate the new gear based on the gear change direction
        int newGear = currentGear + gearChangeDirection;

        // Clamp the new gear value between the minimum and maximum gears
        newGear = Mathf.Clamp(newGear, 1, maxGears);

        // Check if the speed is within the acceptable range for upshifting gears
        // use base speeds instead?
        /*if (gearChangeDirection == 1 && speed >= gearMinSpeeds[newGear - 1] && (newGear == maxGears || speed <= gearMaxSpeeds[newGear - 1]))
        {
            // Set the current gear to the new gear
            currentGear = newGear;
        }*/

        if(gearChangeDirection == 1)
        {
            if(speed >= gearMinSpeeds[newGear - 1])
            {
                currentGear = newGear;
            }
        }
        // Allow downshifting gears without checking the speed
        else if (gearChangeDirection == -1)
        {
            // Set the current gear to the new gear
            currentGear = newGear;
        }
    }

    public float getGearMax()
    {
        return gearMaxSpeeds[currentGear - 1];
    }

    // this prevents things from arising when the maxSpeed is temporarily altered and needs increased
    void increaseBaseMaxSpeed(float _diff)
    {

        // adjust each gear appropriately
        for(int i = 0; i < maxGears; ++i)
        {
            float gearDiff = baseMaxSpeeds[i] - baseMinSpeeds[i];

            gearMaxSpeeds[i] = (gearMaxSpeeds[i] / baseMaxSpeeds[i]) * (baseMaxSpeeds[i] + _diff);
            baseMaxSpeeds[i] += _diff;
            baseMinSpeeds[i] = baseMaxSpeeds[i] - gearDiff;
            gearMinSpeeds[i] = baseMinSpeeds[i];
        }

    }

    // reset max speeds to their default
    void resetMaxSpeeds()
    {
        for(int i = 0; i < maxGears; ++i)
        {
            gearMaxSpeeds[i] = baseMaxSpeeds[i];
        }
    }

    // temporary increase to speed
    void multMaxSpeed(float _factor)
    {
        for(int i = 0; i < maxGears;++i)
        {
            gearMaxSpeeds[i] = Mathf.Max(gearMinSpeeds[i] + 1, gearMaxSpeeds[i] * _factor);
        }
    }

    // temporary increase to speed
    void addMaxSpeed(float _diff)
    {
        for(int i = 0; i < maxGears; ++i)
        {
            gearMaxSpeeds[i] = Mathf.Max(gearMinSpeeds[i] + 1, gearMaxSpeeds[i] += _diff);
        }
    }

    // handle how the player interacts with liquids
    //this was added from cows liquids and length
    private void handleLiquids()
    {
        // deactivate amphibious effect if enough time has elapsed
        if (ampActive && ampStart >= 0 && Time.time - ampStart > ampTime)
        {
            GameObject boostPart = transform.Find("SpeedEffect_ParticleSystem").gameObject;
            if (boostPart != null)
                boostPart.SetActive(false);
            else
                print("Particle not found");
            ampActive = false;
            ampStart = -1;
            speed /= 1.2f;
            resetMaxSpeeds();
        }

        // see if the player is over liquid
        if (checkBelow(LayerMask.GetMask("Liquid")))
        {
            if (!prevLiquid)    // the player was not over a liquid the last frame
            {
                if (!amphibious)        // player does not have amphibious upgrade and should be slowed
                {
                    multMaxSpeed(.8f);
                    speed -= 10;
                    prevLiquid = true;
                }
                else if (!ampActive)    // player has amphibious and it has not been activated
                {
                    GameObject boostPart = transform.Find("SpeedEffect_ParticleSystem").gameObject;
                    if (boostPart != null)
                        boostPart.SetActive(true);
                    else
                        print("Particle not found");
                    //maxSpeed *= 1.2f;

                    multMaxSpeed(1.2f);
                    speed *= 1.2f;
                    ampActive = true;
                    prevLiquid = true;

                }
                else if (ampActive)     // player has amphibious and it is active
                {

                    // deactivates timer
                    prevLiquid = true;
                    ampStart = -1;
                }
            }
        }
        else
        {
            if (prevLiquid)     // the player was over a liquid the last frame
            {
                if (!amphibious)    // the player does not have the amphibious upgrade
                {
                    resetMaxSpeeds();
                    prevLiquid = false;
                }
                else                // the player does have the amphibious upgrade
                {
                    ampStart = Time.time;
                    prevLiquid = false;
                }
            }
        }

        // this should only slow down the player if needed
        if (prevLiquid)
        {
            speed = Mathf.Max(gearMaxSpeeds[currentGear - 1], speed - (1000 * Time.deltaTime));
        }
    }

    // increases max speed
    public void upgradeEngine(int inc)
    {
        //addMaxSpeed(inc);
        increaseBaseMaxSpeed(inc);
    }

    // reduces gas prices
    public void upgradeTank(float mult)
    {
        GetComponent<GasSystem>().purchasePrice *= mult;
    }

    // reduces repair prices
    public void upgradeGreaseMonkey(float mult)
    {
        gameObject.GetComponent<PlayerHealth>().repairCost *= mult;
    }

    // armor accessor
    public bool getArmor()
    {
        return armored;
    }

    // immunity to snipers, slower movement
    public void upgradeArmor()
    {
        armored = true;
        //addMaxSpeed(-30f);
        increaseBaseMaxSpeed(-30f);
    }

    // amphibius accessor
    public bool getAmphibious()
    {
        return amphibious;
    }

    // slower 'ground' traversal, faster liquid traversal
    public void upgradeAmphibious()
    {
        amphibious = true;
        //addMaxSpeed(-5f);
        increaseBaseMaxSpeed(-5f);
    }

    public bool getSponsor()
    {
        return sponsored;
    }

    // the player can regenerate health at the cost of ads
    public void upgradeAl()
    {
        sponsored = true;
        PlayerHealth healthInfo = gameObject.GetComponent<PlayerHealth>();
        healthInfo.regenRate = 5f;
        healthInfo.regenPercent = healthInfo.maxHealth * .05f;
        healthInfo.regen = true;
        alTimer = Time.time;
        alTime = 200f + (Random.Range(0f, 1f) * 300f);
    }

    public bool getRamp()
    {
        return rampedUp;
    }

    // slower 'ground' traversal, faster liquid traversal
    public void upgradeRamp()
    {
        rampedUp = true;
        // Damage & Health system not fully implemented yet
    }

    private bool checkBelow(LayerMask _layer)
    {
        // see if there is liquid below the player
        bool rlt = Physics.Raycast(rayPos, Vector3.down, .52f, _layer); //.52 may be too strict

        return rlt;
    }

    private bool groundCheck(LayerMask _layer)
    {
        RaycastHit hit;
        bool rlt = Physics.Raycast(transform.position, -transform.up, out hit, 1f, _layer);
        return rlt; 
    }

    private bool carFlipped()
    {
        if((Vector3.Dot(transform.up, Vector3.down) > .8))
        { return true; }
        else
        { return false; }
    }


    public void controlLanding()
    {
        if(CarAttachRamp.carAttach)
        {
            if (groundCheck(LayerMask.GetMask("Default")) || checkBelow(LayerMask.GetMask("Liquid")))
            {
                rb.useGravity = true;
                if (landing)
                {
                    if (speed <= gearMinSpeeds[0] + 10f)
                    {
                        StartCoroutine(LandingCar());
                        CarAttachRamp.carAttach = false;
                        landing = false;
                    }
                }
                rb.drag = 0.02f;
                rb.angularDrag = 0.05f;
            }
            else
            {
                rb.useGravity = false;
                rb.drag = 1.05f;
                rb.angularDrag = 1f;
                rb.AddForce(Physics.gravity * rb.mass);
                speed = gearMinSpeeds[0];
                RaycastHit hit;
                bool rlt = Physics.Raycast(transform.position, -transform.up, out hit, LayerMask.GetMask("Default"));
                var targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
                landing = true;
            }
            
        }
    }

    IEnumerator LandingCar()
    {
        landing = true;
        while (speed < gearMinSpeeds[0] + 10)
            speed += 1f;
        yield return new WaitForSeconds(1f);
    }
}
