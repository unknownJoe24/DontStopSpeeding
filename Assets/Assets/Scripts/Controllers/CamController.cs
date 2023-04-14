using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private GameObject target;                      // What will the camera focus on
    private Vector3 offset;                         // Where is the camera's offset from `target`
    private Vector3 baseRotation;                   // What is the rotation of the camera
    private bool specialMove;                       // Will the camera exhibit special behavior
    private VehicleBehaviour.WheelVehicle carInfo;  // Where to get the information of the car
    private float minTurn;                          // The minimum angle that will cause a drift effect
    private float yMove;                            // How much will the camera rotate when the car drifts
    private float carSpeed;                         // How fast is the player moving


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        offset = new Vector3(0f, 5f, -7f);
        baseRotation = new Vector3(15f, 0f, 0f);
        transform.position = target.transform.position + offset;
        transform.rotation = Quaternion.Euler(baseRotation);

        specialMove = true;
        carInfo = target.GetComponent<VehicleBehaviour.WheelVehicle>();
        minTurn = 15f;
        yMove = 5f;
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
        transform.position = target.transform.position + offset + new Vector3(0f, 0f, - Mathf.Log(Mathf.Max(0.1f, carSpeed)));
        //transform.position = target.transform.position + offset + new Vector3(0f, 0f, -Mathf.Min(carSpeed / 10f, 2.5f));
    }

    // Rotates the yaw of the camera by yMove when the player is turning
    public void driftEffect()
    {
        if (specialMove)
        {
            bool drift = Input.GetButton("Drift");
            float angle = target.transform.rotation.eulerAngles.y;

            if (drift)
            {
                if (angle > 180f + minTurn && angle < 360f - minTurn)  // Left
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(baseRotation + new Vector3(0f, -yMove, 0f)), .1f);
                }
                else if (angle > minTurn && angle < 180f - minTurn) // Right
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(baseRotation + new Vector3(0f, yMove, 0f)), .1f);
                }
                else
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(baseRotation), .1f);
                }
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(baseRotation), .1f);
            }
        }
    }
}
