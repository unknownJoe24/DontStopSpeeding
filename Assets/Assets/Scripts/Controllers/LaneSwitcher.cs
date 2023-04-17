using UnityEngine;

public class LaneSwitcher : MonoBehaviour
{
    public float laneWidth = 10f;            // Width of each lane
    public float moveSpeed = 5f;            // Speed at which player moves forward
    public float laneChangeSpeed = 50f;      // Speed at which player changes lanes visually
    public float snapThreshold = 0.01f;     // Distance threshold to snap to target lane

    private Rigidbody rb;
    private int currentLane = 0;            // Current lane, starts in the middle lane (lane 1)
    private int targetLane = 0;             // Target lane, starts in the middle lane (lane 1)

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
    }

    void Update()
    {
        gearChange = Input.GetButtonDown("Change Gear");
        defuseBomb = Input.GetButtonDown("Defuse Bomb");
        repairCar = Input.GetButtonDown("Repair Car");

        upgradeOne = Input.GetButtonDown("Upgrade One");
        upgradeTwo = Input.GetButtonDown("Upgrade Two");
        upgradeThree = Input.GetButtonDown("Upgrade Three");

        // If input is positive, move to the right lane
        if (Input.GetKeyDown(KeyCode.D) && currentLane < 1)
        {
            targetLane = currentLane + 1;
        }
        // If input is negative, move to the left lane
        else if (Input.GetKeyDown(KeyCode.A) && currentLane > -1)
        {
            targetLane = currentLane - 1;
        }

        if (gearChange)
        {
            float gearSetting = Input.GetAxis("Change Gear");

            if (gearSetting > 0)
            {
                GearUp();
            }
            else if (gearSetting < 0)
            {
                GearDown();
            }
        }
        /*
        if (defuseBomb)
        {
            Defuse();
        }

        if (repairCar)
        {
            Repair();
        }

        if (upgradeOne)
        {
            PurchaseUpgradeOne();
        }

        if (upgradeTwo)
        {
            PurchaseUpgradeTwo();
        }

        if (upgradeThree)
        {
            PurchaseUpgradeThree();
        }
        */
        // handle amphibious
        if(ampActive && Time.time - ampStart > ampTime)
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

        // Move the player visually to the target lane
        Vector3 targetPosition = new Vector3(targetLane * laneWidth, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * laneChangeSpeed);

        // Check distance to target lane and snap if within threshold
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < snapThreshold)
        {
            transform.position = targetPosition;
            currentLane = targetLane;
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
    /*
    void Defuse()
    {
        Debug.Log("Defused Bomb");
    }

    void Repair()
    {
        Debug.Log("Repaired Car");
    }


    void PurchaseUpgradeOne()
    {
        //Debug.Log("Purchased Upgrade One");
    }
    void PurchaseUpgradeTwo()
    {
        //Debug.Log("Purchased Upgrade Two");
    }
    void PurchaseUpgradeThree()
    {
        //Debug.Log("Purchased Upgrade Three");
    }
    */

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

        // if Ramped Up, jump
        if (collision.gameObject.CompareTag("Ramp") && rampedUp)
        {
            // Ramp movement & animation
        }
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
