using UnityEngine;

public class LaneSwitcher : MonoBehaviour
{
    public float laneWidth = 1f;            // Width of each lane
    public float moveSpeed = 5f;            // Speed at which player moves forward
    public float laneChangeSpeed = 5f;      // Speed at which player changes lanes visually

    private Rigidbody rb;
    private int currentLane = 1;            // Current lane, starts in the middle lane (lane 1)
    private int targetLane = 1;             // Target lane, starts in the middle lane (lane 1)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Move the player forward
        rb.velocity = new Vector3(0f, 0f, moveSpeed);

        // Get input from the horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //Debug.Log("Horiz" + horizontalInput);

        // If input is positive, move to the right lane
        if (Input.GetKeyDown(KeyCode.D) && currentLane < 2)
        {
            Debug.Log("Current lane pos" + currentLane);
            targetLane = currentLane + 1;
        }
        // If input is negative, move to the left lane
        else if (Input.GetKeyDown(KeyCode.A) && currentLane > 0)
        {
            Debug.Log("Current lane neg" + currentLane);
            targetLane = currentLane - 1;
        }

        // Move the player visually to the target lane
        Vector3 targetPosition = new Vector3(targetLane * laneWidth, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * laneChangeSpeed);

        // If the player is close enough to the target lane, snap to the lane and update the current lane
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            currentLane = targetLane;
        }
    }
}