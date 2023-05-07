using System.Collections;
using UnityEngine;

public class SniperScript : MonoBehaviour
{
    public Transform sniper;
    private Transform player;
    public LineRenderer lineRenderer;
    public PlayerHealth playerHealth;
    private LaneSwitcher carInfo;  // get the script that has the car information

    private bool hasFired = false;
    private float time;

    void Start()
    {
        // initialize the variables storing the car(player) and score info
        carInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<LaneSwitcher>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        lineRenderer.positionCount = 2;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.alignment = LineAlignment.View;
    }

    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if (hasFired)
        {
            lineRenderer.enabled = false;
            return;
        }
        // Wait for 1 second
        time += Time.deltaTime;

        if ((time % 1) == 0 && time >= 3.0f)
        {
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
        }
        else
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }

        if (time >= 4f)
        {

            // Disable the SniperScript component
            enabled = false;

            // Fire the sniper
            Fire();
        }

        lineRenderer.SetPosition(0, sniper.position);
        lineRenderer.SetPosition(1, player.position);
    }

    void Fire()
    {
        print("Fire!");
        if (!carInfo.armored)
        {
            print("Set health to 1");
            playerHealth.setHealth(1);
        }
        else
        {
            playerHealth.damagePlayer(1);
            print("damage player by 1");
        }
            

        hasFired = true;
    }
}