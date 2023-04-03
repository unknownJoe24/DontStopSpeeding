using UnityEngine;

public class LaneSwitcher : MonoBehaviour
{
    public float moveSpeed = 5f;            // Speed at which player moves forward
    private Rigidbody rb;

    public Transform[] lanes;
    private int currentLane = 1;
    private bool isChangingLane = false;
    private int targetLane = 1;

    [Header("State Checks")]
    public bool gearChange = false;
    public bool defuseBomb = false;
    public bool repairCar = false;

    public bool upgradeOne = false;
    public bool upgradeTwo = false;
    public bool upgradeThree = false;

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

        // Check if the player is moving horizontally and change the target lane accordingly
        if (Input.GetAxisRaw("Horizontal") < 0 && currentLane > 0 && !isChangingLane)
        {
            Debug.Log("HZ A");
            targetLane = currentLane - 1;
            isChangingLane = true;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0 && currentLane < lanes.Length - 1 && !isChangingLane)
        {
            Debug.Log("HZ D");
            targetLane = currentLane + 1;
            isChangingLane = true;
        }

        // Move the player to the center of the current or target lane
        Vector3 targetPosition = lanes[isChangingLane ? targetLane : currentLane].position;
        targetPosition.y = transform.position.y;
        targetPosition.z = transform.position.z;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the player has reached the target lane and stop changing lanes
        if (isChangingLane && Mathf.Approximately(transform.position.x, targetPosition.x))
        {
            currentLane = targetLane;
            isChangingLane = false;
        }

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
    void GearUp()
    {
        maxSpeed += 20;
    }

    void GearDown()
    {
        maxSpeed -= 20;
    }

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
        Debug.Log("Purchased Upgrade One");
    }
    void PurchaseUpgradeTwo()
    {
        Debug.Log("Purchased Upgrade Two");
    }
    void PurchaseUpgradeThree()
    {
        Debug.Log("Purchased Upgrade Three");
    }

}
