using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampController : MonoBehaviour
{

    private LaneSwitcher playerInfo;                    // Reference to the WheelVehicle script

    private bool _rampedUp;
    public bool testTrigger;
    private GameObject _player; 
    public GameObject[] points;
    private float currentLerpTime;
    public float speed; 
    private bool isLerping;
    [SerializeField] float lerpTime = 1f;   // Time it takes to get to the position - acts a speed


    public static int pointCounter;
    public static int pointLength;

    private void Start()
    {
        pointCounter = 0;
        pointLength = points.Length;
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Awake()
    {
        _rampedUp = LaneSwitcher.rampedUp;
        testTrigger = false; 
    }

    private void Update()
    {
       

        _rampedUp = LaneSwitcher.rampedUp;

        if(_rampedUp || testTrigger)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            currentLerpTime = 0f;
            if(pointCounter <= points.Length - 1)
            {
                LerpToPoints();
            }
            else
            {
                var _playerRB = _player.GetComponent<Rigidbody>();
                _playerRB.useGravity = true;
                _playerRB.constraints = RigidbodyConstraints.None; this.enabled = false;
            
            }
            
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }


        //print(testTrigger);
    }


    private void SpawnRamps()
    {
        if(_rampedUp || testTrigger)
        {
            //print("Ramp Should Spawn");
            transform.GetChild(0).gameObject.SetActive(true);
        }

        //testing
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }

    private void LerpToPoints()
    {
            currentLerpTime += Time.deltaTime;
        /* if (currentLerpTime > lerpTime)
         {

             currentLerpTime = lerpTime;
             //isLerping = false;
         }*/
            var step = speed * Time.deltaTime; // calculate distance to move
            float percentComplete = currentLerpTime / lerpTime;
            var _playerRB = _player.GetComponent<Rigidbody>();
            _playerRB.useGravity = false;
            _playerRB.constraints = RigidbodyConstraints.FreezeRotation;
            _playerRB.MovePosition(Vector3.MoveTowards(_player.transform.position, points[pointCounter].transform.position, step));
            //_player.transform.LookAt(points[pointCounter].transform.position);

        //move to + look at
    }

}
