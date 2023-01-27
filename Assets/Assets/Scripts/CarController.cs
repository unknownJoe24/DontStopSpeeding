using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float speedIncrement = 10f;
    public float maxSpeed = 50f;
    public float turnSpeed = 25f;
    public float handbrakeSpeed = 100f;
    public Rigidbody rb;

    [Header("Car Stats")]
    public float currentSpeed;

    [Header("State Checks")]
    public bool accelerate = false;
    public bool brake = false;
    public float handbrake = 0.0f;
    public bool gearChange = false;
    public bool defuseBomb = false;
    public bool repairCar = false;
    public bool gas = false;
    public bool upgradeOne = false;
    public bool upgradeTwo = false;
    public bool upgradeThree = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = 0f;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        accelerate = Input.GetButtonDown("Accelerate");
        brake = Input.GetButtonDown("Brake");
        handbrake = Input.GetAxis("Handbrake");
        gearChange = Input.GetButtonDown("Change Gear");
        defuseBomb = Input.GetButtonDown("Defuse Bomb");
        repairCar = Input.GetButtonDown("Repair Car");
        gas = Input.GetButtonDown("Fuel Car");
        upgradeOne = Input.GetButtonDown("Upgrade One");
        upgradeTwo = Input.GetButtonDown("Upgrade Two");
        upgradeThree = Input.GetButtonDown("Upgrade Three");

        if (accelerate)
        {
            currentSpeed = Mathf.Clamp(currentSpeed + speedIncrement, 0f, maxSpeed);
        }

        if (brake)
        {
            currentSpeed = Mathf.Clamp(currentSpeed - speedIncrement, 0f, maxSpeed);         
        }

        // Apply handbrake
        if (handbrake > 0)
        {
            // Apply a small amount of opposite force to slow the car down when handbrake is engaged
            rb.AddForce(-transform.forward * 0.025f * currentSpeed , ForceMode.VelocityChange);
            Drift(horizontalInput, 0.05f);
        }

        if (gearChange)
        {
            float gearSetting = Input.GetAxis("Change Gear");

            if (gearSetting > 0)
            {
                GearUp();
            }
            else if( gearSetting < 0)
            {
                GearDown();
            }
        }

        if (defuseBomb)
        {
            Defuse();
        }

        if (repairCar)
        {
            Repair();
        }

        if (gas)
        {
            Gas();
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

        // Turn the car left or right
        transform.Rotate(Vector3.up, horizontalInput * turnSpeed * Time.deltaTime);
        // Move car forward
        rb.velocity = transform.forward * currentSpeed;
    }

    void FixedUpdate()
    {
        if (currentSpeed > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        Drift(horizontalInput, 0.15f);
    }

    void Drift(float horizontalInput, float scale)
    {
        rb.AddForce(-transform.right * scale * horizontalInput * turnSpeed * scale * currentSpeed * scale , ForceMode.VelocityChange);
        // Apply torque to the front wheels
        rb.AddTorque(transform.forward * horizontalInput * turnSpeed * scale * currentSpeed, ForceMode.VelocityChange);
        // Apply opposite torque to the back wheels
        rb.AddTorque(-transform.forward * horizontalInput * turnSpeed * scale * currentSpeed, ForceMode.VelocityChange);
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

    void Gas()
    {
        Debug.Log("Purchased Gas");
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