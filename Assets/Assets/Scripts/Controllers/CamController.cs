using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private GameObject target;                      // What will the camera focus on

    [SerializeField] private Vector3 offset;                         // Where is the camera's offset from `target`
    private Vector3 baseRotation;                   // What is the rotation of the camera
    private LaneSwitcher carInfo;                   // Where to get the information of the car
    private float carSpeed;                         // How fast is the player moving


    // Start is called before the first frame update
    void Start()
    {
        // initialize target and the camera information
        target = GameObject.FindGameObjectWithTag("Player");
        offset = new Vector3(0f, 8f, -9f); //further back
        baseRotation = new Vector3(15f, 0f, 0f);
        transform.position = target.transform.position + offset;
        transform.rotation = Quaternion.Euler(baseRotation);

        // initialize special movement information
        carInfo = target.GetComponent<LaneSwitcher>();
        carSpeed = carInfo.Speed;
    }

    // Update is called once per frame
    void Update()
    {
        // get the car speed and handle the drife effect
        carSpeed = carInfo.Speed;
    }

    private void LateUpdate()
    {
        transform.position = target.transform.position + offset + new Vector3(0f, 0f, - Mathf.Log(Mathf.Max(0.1f, carSpeed)));
    }

    // changes the target of the camera
    public void updateTarget(GameObject _target)
    {
        target = _target;
        carSpeed = carInfo.Speed; 
        transform.position = target.transform.position + offset;
        transform.rotation = Quaternion.Euler(baseRotation);
    }

    public void updateOffset(Vector3 newOffset)
    {
        offset = offset += newOffset; 
    }
}
