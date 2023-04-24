using UnityEngine;

public class LaneSwitcher : MonoBehaviour
{
    public float moveSpeed = 5f;            // Speed at which player moves forward
    private Rigidbody rb;
    private Vector3 rayPos;                 // where does the ray that checks beneatht the car come from

    public Transform[] twoLaneTransforms;
    public Transform[] threeLaneTransforms;
    private int currentLane = 1;
    private bool isChangingLane = false;
    private int targetLane = 1;
    public bool hasThreeLanes = true;

    [Header("State Checks")]
    public bool gearChange = false;
    public bool defuseBomb = false;
    public bool repairCar = false;

    public bool upgradeOne = false;
    public bool upgradeTwo = false;
    public bool upgradeThree = false;

    [SerializeField]
    public bool armored = false;
    [SerializeField]
    private bool prevLiquid = false;
    public bool amphibious = false;
    private bool ampActive;
    public float ampTime = 3f;
    private float ampStart;
    [SerializeField]
    public bool sponsored = false;
    private float alTimer;
    private float alTime;
    [SerializeField]
    public bool rampedUp = false;

    [Header("Car Settings")]
    public float speedIncrement = 10f;
    public float maxSpeed = 50f;
    private float baseMaxSpeed;
    public float speedInc;
    private float sinceInc;
    private float minSpeed;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    [SerializeField] float speed = 0.0f;    // Use this to read the current car speed (you'll need this to make a speedometer)
    public float Speed => speed;

    [SerializeField] bool disableMovement;
    public bool DisableMovement
    {
        get => disableMovement;
        set => disableMovement = value;
    }

    void Start()
    {
        alTime = 200f + (Random.Range(0f, 1f) * 300f);
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0f, 0f, moveSpeed);       // Set initial movement velocity of RB

        ampStart = -1f;
        baseMaxSpeed = maxSpeed;

        sinceInc = 0f;
        minSpeed = 0f;
    }

    void Update()
    {
        rayPos = gameObject.transform.position;
        rayPos.z = GameObject.Find("FLWheel").transform.position.z;

        gearChange = Input.GetButtonDown("Change Gear");
        defuseBomb = Input.GetButtonDown("Defuse Bomb");
        repairCar = Input.GetButtonDown("Repair Car");

        upgradeOne = Input.GetButtonDown("Upgrade One");
        upgradeTwo = Input.GetButtonDown("Upgrade Two");
        upgradeThree = Input.GetButtonDown("Upgrade Three");

        // Check if the player is moving horizontally and change the target lane accordingly

        if (Input.GetAxisRaw("Horizontal") < 0 && currentLane > 0 && !isChangingLane)
        {
            targetLane = currentLane - 1;
            isChangingLane = true;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0 && currentLane < (hasThreeLanes ? threeLaneTransforms.Length : twoLaneTransforms.Length) - 1 && !isChangingLane)
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
    
        // handle amphibious
        if (ampActive && ampStart >= 0 && Time.time - ampStart > ampTime)
        {
            ampActive = false;
            speed /= 1.2f;
            maxSpeed = baseMaxSpeed;
        }

        // handle Better Call Al's sponsorship segment
        if(sponsored && Time.time - alTimer > alTime)
        {
            // Better Call Al!
            alTimer = Time.time;
            alTime = 200f + (Random.Range(0f, 1f) * 300f);
        }

        if(checkBelow(LayerMask.GetMask("Liquid")))
        {
            if(!prevLiquid)
            {
                if (!amphibious)
                {
                    maxSpeed *= .8f;     //BIND????
                    speed -= 10;       // slow down faster
                    prevLiquid = true;
                }
                else if(!ampActive)
                {
                    maxSpeed *= 1.2f;
                    speed *= 1.2f;
                    ampActive = true;
                    prevLiquid = true;
                }
                else if(ampActive)
                {
                    // deactivates timer
                    prevLiquid = true;
                    ampStart = -1;
                }
            }
        }
        else
        {
            if(prevLiquid)
            {
                if (!amphibious)
                {
                    maxSpeed = baseMaxSpeed;
                    prevLiquid = false;
                }
                else
                {
                    ampStart = Time.time;
                    prevLiquid = false;
                }
            }
        }

        if(prevLiquid)
        {
            speed = Mathf.Max(maxSpeed, speed - (1000 * Time.deltaTime)); // slow down faster than friction
        }

        handleMinSpeed();
    }

    private void FixedUpdate()
    {
        speed = transform.InverseTransformDirection(rb.velocity).z;

        if (speed < maxSpeed && !disableMovement)
        {
            rb.AddForce(transform.forward * 10000);
        }

        if (disableMovement && speed > 0.0f)
        {
            rb.drag += 0.05f;
        }

    }

    // handles the increase in the minimum speed and kills the player if the fall below it
    void handleMinSpeed()
    {
        sinceInc += Time.deltaTime;
        if (sinceInc >= speedInc)
        {
            increaseMaxSpeed(10f);
            minSpeed += 10;
            sinceInc = 0f;
        }

        PlayerHealth healthInfo = gameObject.GetComponent<PlayerHealth>();
        BombDefusal bombInfo = GameObject.Find("DefusalHandler").GetComponent<BombDefusal>();
        
        if (speed < minSpeed && !healthInfo.dead && !bombInfo.getCompleted())
        {
            healthInfo.killPlayer();
            Debug.Log("The player's speed: " + speed.ToString() + "\nThe Min Speed: " + minSpeed.ToString());
        }
    }
    void GearUp()
    {
        maxSpeed += 20;
    }

    void GearDown()
    {
        maxSpeed -= 20;
    }

    // this prevents things from arising when the maxSpeed is temporarily altered and needs increased
    void increaseMaxSpeed(float _diff)
    {
        maxSpeed = (maxSpeed / baseMaxSpeed) * (baseMaxSpeed + _diff);
        baseMaxSpeed += _diff;
    }



    // upgrade implementation

    // increases max speed
    public void upgradeEngine(int inc)
    {
        maxSpeed += inc;
    }

    // reduces gas prices
    public void upgradeTank(float mult)
    {
        GetComponent<GasSystem>().purchasePrice *= mult;
    }

    // reduces repair prices
    public void upgradeGreaseMonkey(float mult)
    {
        // There isn't really yet a way to implement this yet
        Debug.Log("Purchased Grease Monkey...  Nice. \n");
    }

    public bool getArmor()
    {
        return armored;
    }

    // immunity to snipers, slower movement
    public void upgradeArmor()
    {
        armored = true;
        maxSpeed -= 30f;
    }

    public bool getAmphibious()
    {
        return amphibious;
    }

    // slower 'ground' traversal, faster liquid traversal
    public void upgradeAmphibious()
    {
        amphibious = true;
        maxSpeed -= 5f;
    }

    public bool getSponsor()
    {
        return sponsored;
    }

    // slower 'ground' traversal, faster liquid traversal
    public void upgradeAl()
    {
        sponsored = true;
        // Damage & Health system not fully implemented yet
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
        bool rlt = Physics.Raycast(rayPos, Vector3.down, 20f, _layer);
        return rlt;
    }
}
