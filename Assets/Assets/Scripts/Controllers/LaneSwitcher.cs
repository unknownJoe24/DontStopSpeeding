using UnityEngine;

public class LaneSwitcher : MonoBehaviour
{
    public float moveSpeed = 5f;            // Speed at which player moves forward
    private Rigidbody rb;

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
    public float speedInc;
    private float sinceInc;

    [Header("Gear Settings")]
    public int currentGear = 1;
    public int maxGears = 3;
    public float[] gearMinSpeeds = { 50f, 70f, 100f };
    public float[] gearMaxSpeeds = { 70f, 100f, 120f };

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

        sinceInc = 0f;

        // Set the initial minimum speed based on the current gear
        minSpeed = gearMinSpeeds[currentGear - 1];

        // Set the initial speed to be equal to the initial minimum speed
        speed = minSpeed;

        // Set the initial movement velocity of RB
        rb.velocity = new Vector3(0f, 0f, speed);
    }

    void Update()
    {
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

        // Gear change
        if (gearChange)
        {
            ChangeGear();
        }

        // handle amphibious
        if (ampActive && Time.time - ampStart > ampTime)
        {
            ampActive = false;
            Debug.Log("Deactivating amphibious");
            speed /= 1.2f;
            maxSpeed /= 1.2f;
        }

        // handle Better Call Al's sponsorship segment
        if(sponsored && Time.time - alTimer > alTime)
        {
            // Better Call Al!
            alTimer = Time.time;
            alTime = 200f + (Random.Range(0f, 1f) * 300f);
        }

        handleMinSpeed();
        AdjustSpeed();
    }

    private void FixedUpdate()
    {
        speed = transform.InverseTransformDirection(rb.velocity).z;

        if (!disableMovement)
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
            maxSpeed += 10;
            minSpeed += 10;
            sinceInc = 0f;
        }

        // Get the minimum speed for the current gear
        float gearMinSpeed = gearMinSpeeds[currentGear - 1];

        PlayerHealth healthInfo = gameObject.GetComponent<PlayerHealth>();
        BombDefusal bombInfo = GameObject.Find("DefusalHandler").GetComponent<BombDefusal>();

        // Check if the speed is less than the minimum speed for the current gear
        if (speed < gearMinSpeed && !healthInfo.dead && !bombInfo.getCompleted())
            healthInfo.killPlayer();
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
            speed = Mathf.Lerp(speed, gearMaxSpeed, 0.05f);

            // Update the rigidbody's velocity with the adjusted speed while keeping the x and y components unchanged
            Vector3 newVelocity = rb.velocity;
            newVelocity.z = speed;
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
        if (gearChangeDirection == 1 && speed >= gearMinSpeeds[newGear - 1] && (newGear == maxGears || speed <= gearMaxSpeeds[newGear - 1]))
        {
            // Set the current gear to the new gear
            currentGear = newGear;
        }
        // Allow downshifting gears without checking the speed
        else if (gearChangeDirection == -1)
        {
            // Set the current gear to the new gear
            currentGear = newGear;
        }

        // Set the new minSpeed based on the current gear
        minSpeed = gearMinSpeeds[currentGear - 1];
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

    // upgrade effects
    private void OnCollisionEnter(Collision collision)
    {
        // if amphibious, get speed boost
        if(collision.gameObject.CompareTag("Liquid") && amphibious)
        {
            maxSpeed *= 1.2f;
            speed *= 1.2f;
            ampActive = true;
            Debug.Log("Look at that boost!");
        }
        
        /*
        // if Ramped Up, jump
        if (collision.gameObject.CompareTag("Ramp") && rampedUp)
        {
            // Ramp movement & animation
        }
        */
    }

    private void OnCollisionExit(Collision collision)
    {
        // take note of leaving liquid
        if (collision.gameObject.CompareTag("Liquid") && amphibious)
        {
            ampStart = Time.time;
        }
    }

}
