using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{

    public float health;                                // current player health
    public float invulnerableDur;                       // how long the player becomes invulnerable
    public bool defense = false;                        // defense - redundant?
    public bool regen = false;                          // does the car regenerate
    public float regenRate;

    float currentSpeed;                                 // current speed player is going

    private float invulnerableTime;                     // the time since invulnerability began
    private float second;                               // the time since health was regenerated
    private LaneSwitcher playerInfo;                    // Reference to the WheelVehicle script
    private Rigidbody playerRigidBody;                  // the rigid body of the player
    private TMP_Text healthText;

    // Start is called before the first frame update
    void Start()
    {
        health = 50;                                    // sets player health at start
        invulnerableDur = 2f;
        regenRate = 1f;

        invulnerableTime = 0f;
        second = 0.0f;

        playerInfo = gameObject.GetComponent<LaneSwitcher>();
        playerRigidBody = gameObject.GetComponent<Rigidbody>();
        healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // get the player's current speed
        currentSpeed = playerInfo.Speed;

        // redisplay the health HUD
        redisplayInfo();

        // handle invulnerability
        if (gameObject.layer == LayerMask.NameToLayer("Invincible"))
        {
            invulnerableTime += Time.deltaTime;
            // make the player vulnerable again
            if(invulnerableTime >= invulnerableDur)
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
                invulnerableTime = 0f;
            }
        }

        // recover health if the player has taken damage
        if(regen && health > 0)
        {
            recoverHealth();
        }

        // kill the player if they ran out of health
        if(health <= 0)
        {
            //die
            killPlayer();
        }

        // debugging
        if(Input.GetKeyDown("k"))       // this is temporary
        {
            killPlayer();
        }
    }

    // Recover player health over time
    void recoverHealth()
    {
        // recover 5 health every regenRate seconds
        // get time and then check every regenRate seconds
        if(health < 100 && regen)
        {
            second += Time.deltaTime;
            if(second >= regenRate)
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
            health -= currentSpeed / 10;
            handleInvulnerability();
        }
        /*
            if(currentSpeed > 0)
            {
            if(defense)
            {
                float tmpDamage = currentSpeed / 10;
                health -= tmpDamage / 2;
            }
            if(!defense)
            {
                //Current Speed before impact / 10 = damage dealt
                health -= currentSpeed / 10;
            }

        
        }*/
    }

    // updates the health UI
    void redisplayInfo()
    {
        healthText.text = "Health: " + health.ToString();
    }

    // kills the player, prevents movement, and plays the death animation
    void killPlayer()
    {
        // stop movement
        playerInfo.DisableMovement = true;
        playerRigidBody.velocity = new Vector3(0f, 0f, 0f);

        // create child clone
        GameObject newCar = Instantiate(gameObject);
        newCar.transform.parent = transform;

        // enable the animator and play the animation
        Animator playerAnimator = newCar.GetComponent<Animator>();
        playerAnimator.enabled = true;
        playerAnimator.bodyPosition = transform.position;
        playerAnimator.rootPosition = transform.position;
        playerAnimator.applyRootMotion = false;
        playerAnimator.SetBool("Dead", true);
    }

    // change the player's vulnerability
    void handleInvulnerability()
    {

        // get all objects under gameObject that have a collider
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        GameObject[] colliderObjects = new GameObject[allColliders.Length];
        if (allColliders.Length == 0)
            Debug.LogError("The colliders were not achieved properly");

        for(int i = 0; i < allColliders.Length; ++i)
        {
            colliderObjects[i] = allColliders[i].gameObject;
        }

        //set the layer for every object under and including the player that has a collider
        gameObject.layer = LayerMask.NameToLayer("Invincibility");
        for(int i = 0; i < colliderObjects.Length; ++i)
        {
            colliderObjects[i].layer = LayerMask.NameToLayer("Invincibility");
        }
    }

    // checks for collisions with the player (only the collider for the super object (I believe))
    private void OnCollisionEnter(Collision collision)
    {
        // only take damage if the player hits an obstacle
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Damage")
        {
            takeDamage();
        }
    }
}

// stack overflow