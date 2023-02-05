using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private GameObject target;              // What will the camera focus on
    private Vector3 offset;                 // Where is the camera's offset from `target`
    private Vector3 rotation;               // What is the rotation of the camera
    private bool specialMove;               // Will the camera exhibit special behavior
    VehicleBehaviour.WheelVehicle carInfo;  // Where to get the information of the car
    private float yMove;                    // How much will the camera rotate when the car drifts
    float carSpeed;                         // How fast is the player moving


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        offset = new Vector3(0f, 20f, -20f);
        rotation = new Vector3(30f, 0f, 0f);
        transform.position = target.transform.position + offset;
        transform.rotation = Quaternion.Euler(rotation);

        specialMove = true;
        carInfo = target.GetComponent<VehicleBehaviour.WheelVehicle>();
        yMove = 15f;
        carSpeed = carInfo.Speed;
    }

    // Update is called once per frame
    void Update()
    {
        carSpeed = carInfo.Speed;
        driftEffect();
    }

    private void LateUpdate()
    {
        Debug.Log(Mathf.Log(Mathf.Max(0.1f, carSpeed)));
        //transform.position = target.transform.position + offset + new Vector3(0f, 0f, - Mathf.Log(Mathf.Max(0.1f, carSpeed)));
        transform.position = target.transform.position + offset + new Vector3(0f, 0f, -Mathf.Min(carSpeed / 10f, 7.5f));
    }

    // Rotates the yaw of the camera by yMove when the player is turning
    public void driftEffect()
    {
        if (specialMove)
        {
            float turn = Input.GetAxis("Horizontal");
            if (turn < 0f)  // Left
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotation + new Vector3(0f, -yMove, 0f)), .1f);
            }
            else if (turn > 0f) // Right
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotation + new Vector3(0f, yMove, 0f)), .1f);
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotation), .1f);
            }
        }
    }
}
