using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float health;                                // current player health
    public bool dead;                                   // is the player dead 
    public float invulnerableDur;                       // how long the player becomes invulnerable
    public bool defense = false;                        // defense - redundant?
    public bool regen = false;                          // does the car regenerate
    public float regenRate;
    public GameObject player_explosion;                 //this was added in the ToDoListSarah branch as a reimplementation of the car death sequence


    [Header ("Sound Settings")]
    public AudioClip hurtSound;                         // sound when the player is hurt
    public AudioClip deathSound;                        // sound when the player dies



    float currentSpeed;                                 // current speed player is going


    private float invulnerableTime;                     // the time since invulnerability began
    private float second;                               // the time since health was regenerated
    private LaneSwitcher playerInfo;                    
    //private VehicleBehaviour.WheelVehicle playerInfo;   // Reference to the WheelVehicle script
    private Rigidbody playerRigidBody;

    [Header("UI Settings")]
    public TMP_Text healthText;
    public Image healthBar;


    GameObject[] colliderObjects;                       // the individual colliders of the player

    // Start is called before the first frame update
    void Start()
    {
        dead = false; 
        // member initialization
        invulnerableDur = 2f;
        regenRate = 1f;
        invulnerableTime = 0f;
        second = 0.0f;

        //health = 100;                                    // sets player health at start


        playerInfo = gameObject.GetComponent<LaneSwitcher>();
        playerRigidBody = gameObject.GetComponent<Rigidbody>();
        //healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<TMP_Text>();

        // get all objects under gameObject that have a collider
        // this is used to swap the player's layer
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        colliderObjects = new GameObject[allColliders.Length];
        if (allColliders.Length == 0)
            Debug.LogError("The colliders were not achieved properly");

        for (int i = 0; i < allColliders.Length; ++i)
        {
            colliderObjects[i] = allColliders[i].gameObject;
        }

    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed = playerInfo.Speed; //-- THIS LINE DOESN'T WORK(?)

        // redisplay the health HUD
        redisplayInfo();

        // handle invulnerability
        if (gameObject.layer == LayerMask.NameToLayer("Invincibility"))
        {
            invulnerableTime += Time.deltaTime;
            // make the player vulnerable again
            if (invulnerableTime >= invulnerableDur)
            {
                handleInvulnerability(false);
            }
        }

        // recover health if the player has taken damage
        if (regen && health > 0)
            recoverHealth();

        if (health > 0)
        {
                recoverHealth();
        }

        // kill the player if they ran out of health
        if (!dead && health <= 0)
            killPlayer();

        //This was commented out to avoid a Merge Conflict for cows liquids lengths
       /* if (Input.GetKeyDown("k"))       // this is temporary
        {
            takeDamage();
            //print("taking damage");
        }
       */
    }

    // Recover player health over time
    void recoverHealth()
    {
        // recover 5 health every regenRate seconds
        // get time and then check every regenRate seconds
        if (health < 100 && regen)
        {
            second += Time.deltaTime;
            if (second >= regenRate)
            {
                health += 5;
                second = 0;
                if (health > 100)
                {
                    health = 100;
                }
            }
        }
    }

    // Remove health from player when damage is dealt
    void takeDamage()
    {
        if (currentSpeed > 0)
        {
            health -= currentSpeed / 10;
            if (health > 0)
                SoundManager.Instance.Play(hurtSound, 1f);

            handleInvulnerability(true);
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


    public void damagePlayer(int amount)
    {
        health -= amount;
        if (health < 0)
            health = 0;

        if (health > 0)
            SoundManager.Instance.Play(hurtSound, 1f);

        handleInvulnerability(true);
    }

    public void setHealth(int amount)
    {

        float prevHealth = health;
        health = amount;

        if (prevHealth > health)
            SoundManager.Instance.Play(hurtSound, 1f);

        handleInvulnerability(true);
    }

    void redisplayInfo()
    {
        healthText.text = "HP: " + health.ToString();
        healthBar.fillAmount = health / 100;
    }

    // kills the player, prevents movement, and plays the death animation
    public void killPlayer()
    {
        dead = true;

        // stop movement
        playerInfo.DisableMovement = true;
        print("Player health, playerInfo.DisableMovment");
        playerRigidBody.velocity = new Vector3(0f, 0f, 0f);

        //The code below was added in the ToDoListSarah branch as a reimplementation of the car death sequence

        Instantiate(player_explosion, transform.position, Quaternion.identity);
        
        /*
        MeshRenderer[] rs = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer r in rs)
        {
            
            r.enabled = false;
            print(r + " " + r.enabled);

        }
        */
        
        var sn = GameObject.FindObjectOfType<CamController>();
        sn.updateOffset(new Vector3(0, 0, -10f));
        SoundManager.Instance.Play(deathSound, 1f);

        gameObject.SetActive(false); //for some reason, the meshrenderer turning off stopped working. This also works as far as I am aware on this branch (no errors), but might need to be changed when merged with rest of branches
    }

    // change the player's vulnerability
    void handleInvulnerability(bool _status)
    {
        if (_status)
        {
            //set the layer for every object under and including the player that has a collider
            gameObject.layer = LayerMask.NameToLayer("Invincibility");
            for (int i = 0; i < colliderObjects.Length; ++i)
            {
                colliderObjects[i].layer = LayerMask.NameToLayer("Invincibility");
            }
            StartCoroutine(FlashingDamage()); //creates a flashing effect to indicate that the player has taken damage
        }
        else
        {
            //set the layer for every object under and including the player that has a collider
            gameObject.layer = LayerMask.NameToLayer("Default");
            for (int i = 0; i < colliderObjects.Length; ++i)
            {
                colliderObjects[i].layer = LayerMask.NameToLayer("Default");
            }
            invulnerableTime = 0f;
        }
    }


    IEnumerator FlashingDamage()
    {
        //coroutine that gets all the Meshrenderer's of the player and flickers them on and off for aprx. 2 seconds
        MeshRenderer[] rs = GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < 7; i++)
        {
            foreach (MeshRenderer r in rs)
            {
                r.enabled = false;
            }
            SoundManager.Instance.Play(hurtSound, 0.5f);
            yield return new WaitForSeconds(.1f);
            foreach (MeshRenderer r in rs)
            {
                r.enabled = true;
            }
            yield return new WaitForSeconds(.1f);
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
