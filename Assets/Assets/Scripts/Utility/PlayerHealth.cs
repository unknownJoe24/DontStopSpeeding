using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{

    public float maxHealth;
    public float health;                                // current player health
    public bool defense = false;
    public bool regen = false;
    public float regenRate;
    public Animator anim;

    float currentSpeed;                                 // current speed player is going

    private float second;
    private VehicleBehaviour.WheelVehicle playerInfo;   // Reference to the WheelVehicle script
    private Rigidbody playerRigidBody;
    private TMP_Text healthText;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        health = 100;                                    // sets player health at start
        regenRate = 5.0f;
        second = 0.0f;

        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<VehicleBehaviour.WheelVehicle>();
        playerRigidBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed = playerInfo.Speed;

        redisplayInfo();

        if(health > 0)
        {
                recoverHealth();
        }

        if(health <= 0){
            //die
            print("ded");
            //anim.SetBool("death", true);
            anim.Play("Explosion Animation");
            playerInfo.IsPlayer = false;
            playerInfo.Throttle = 0f;
            playerRigidBody.velocity = new Vector3(0f, 0f, 0f);
            
        }

        if(Input.GetKeyDown("k"))       // this is temporary
        {
            takeDamage();
            //print("taking damage");
        }
    }

    // Recover player health over time
    void recoverHealth()
    {
        // recover 5 health every 10 seconds
        // get time and then check every 10 seconds
        if(health < maxHealth && regen)
        {
            second += Time.deltaTime;
            if(second >= 5.0)
            {
                health += 5;
                second = 0;
                if(health > 100)
                {
                    health = 100;
                }
            }
        }
    }

    // Remove health from player when damage is dealt
    void takeDamage()
    {
            if(currentSpeed > 0)
            {
            if(defense)
            {
                float tmpDamage = currentSpeed / 10;
                health -= tmpDamage / 2;
                print("defense damage taken");
            }
            if(!defense)
            {
                //Current Speed before impact / 10 = damage dealt
                health -= currentSpeed / 10;
                print("taking damage");
            }
        }
    }

    void redisplayInfo()
    {
        healthText.text = "Health: " + health.ToString();
    }

    void OnTriggerEnter (Collider other)
    {
        if(other.tag == "testDamage")
        {
            takeDamage();
            print("player collided with object");
        }
    }
}
