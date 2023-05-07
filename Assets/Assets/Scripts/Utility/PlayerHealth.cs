using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth;                             // max health the player can have
    public float health;                                // current player health
    public bool dead;                                   // is the player dead 
    public float invulnerableDur;                       // how long the player becomes invulnerable
    public bool defense = false;                        // defense - redundant?
    public bool regen = false;                          // does the car regenerate

    public GameObject player_explosion;                 //this was added in the ToDoListSarah branch as a reimplementation of the car death sequence

    public float repairCost = 200;                      // the cost of repairing the car
    public float repairPercent;                         // how much does it repair (percentage)
    public float regenPercent;                          // how much health is regenerated
    public float regenRate;                             // how often is health regenerated



    [Header ("Sound Settings")]
    public AudioClip hurtSound;                         // sound when the player is hurt
    public AudioClip deathSound;                        // sound when the player dies



    float currentSpeed;                                 // current speed player is going


    private float invulnerableTime;                     // the time since invulnerability began
    private float second;                               // the time since health was regenerated

    private LaneSwitcher playerInfo;                    // the player information
    [SerializeField]
    private ScoreSystem playerScore;                    // the score system used by the player
    private Rigidbody playerRigidBody;                  // the rigidbody of the player
    [Header("UI Settings")]
    public TMP_Text healthText;                         // the text displaying the health
    public Image healthBar;                             // the bar displaying the health


    GameObject[] colliderObjects;                       // the individual colliders of the player

    // Start is called before the first frame update
    void Start()
    {
        dead = false; 
        invulnerableDur = 2f;
        regenRate = 1f;
        invulnerableTime = 0f;
        second = 0.0f;

        playerInfo = gameObject.GetComponent<LaneSwitcher>();
        playerRigidBody = gameObject.GetComponent<Rigidbody>();

        // get all objects under gameObject that have a collider
        // this is used to swap the player's layer
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        colliderObjects = new GameObject[allColliders.Length];
        for (int i = 0; i < allColliders.Length; ++i)
        {
            colliderObjects[i] = allColliders[i].gameObject;
        }

    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed = playerInfo.Speed;

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

        // recover health if applicable
        recoverHealth();

        // allow the player to repair the car
        if(Input.GetButtonDown("Repair Car") && Time.timeScale != 0)
        {
            if(playerScore.spendMoney(repairCost))
            {
                health += (maxHealth * repairPercent / 100f);
                health = Mathf.Min(health, maxHealth);
            }
        }

        // kill the player if they ran out of health
        if (!dead && health <= 0)
            killPlayer();
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
                health += maxHealth * regenPercent;
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
        if(currentSpeed > 0)
        {

            if(defense)
            {
                float tmpDamage = currentSpeed / 10f;
                health -= tmpDamage / 2f;
                health = Mathf.Max(0f, health);
            }
            else
            {
                //Current Speed before impact / 10 = damage dealt
                health -= currentSpeed / 10f;
                health = Mathf.Max(0f, health);
            }

            if (health > 0)
                SoundManager.Instance.Play(hurtSound, 1f);

            handleInvulnerability(true);
        }
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
