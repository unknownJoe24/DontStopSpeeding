using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private GameObject target;                      // What will the camera focus on
    private Vector3 offset;                         // Where is the camera's offset from `target`
    private Vector3 baseRotation;                   // What is the rotation of the camera
    private bool specialMove;                       // Will the camera exhibit special behavior
    private LaneSwitcher carInfo;                   // Where to get the information of the car
    private float minTurn;                          // The minimum angle that will cause a drift effect
    private float yMove;                            // How much will the camera rotate when the car drifts
    private float carSpeed;                         // How fast is the player moving


    // Start is called before the first frame update
    void Start()
    {
        // initialize target and the camera information
        target = GameObject.FindGameObjectWithTag("Player");
        offset = new Vector3(0f, 5f, -7f);
        baseRotation = new Vector3(15f, 0f, 0f);
        transform.position = target.transform.position + offset;
        transform.rotation = Quaternion.Euler(baseRotation);

        // initialize special movement information
        specialMove = true;
        carInfo = target.GetComponent<LaneSwitcher>();
        minTurn = 15f;
        yMove = 5f;
        carSpeed = carInfo.Speed;
    }

    // Update is called once per frame
    void Update()
    {
        // get the car speed and handle the drife effect
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

        }
    }

    // changes the target of the camera
    public void updateTarget(GameObject _target)
    {
        target = _target;
    }
}
