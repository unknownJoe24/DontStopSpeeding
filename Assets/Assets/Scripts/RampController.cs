using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampController : MonoBehaviour
{

    private LaneSwitcher playerInfo;                    // Reference to the playerInfo speed (might use as basis for deciding speed for moving in LerpToPoints();

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
    public static bool carAttach; 

    private void Start()
    {
        pointCounter = 0;
        pointLength = points.Length;
        carAttach = false; 
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

    private void LerpToPoints()
    {

        if(carAttach)

        {
            currentLerpTime += Time.deltaTime;
            var step = speed * Time.deltaTime; // calculate distance to move
            float percentComplete = currentLerpTime / lerpTime;
            var _playerRB = _player.GetComponent<Rigidbody>();
            _playerRB.useGravity = false;
            _playerRB.constraints = RigidbodyConstraints.FreezeRotation;
            _playerRB.MovePosition(Vector3.MoveTowards(_player.transform.position, points[pointCounter].transform.position, step));
            var rotation = Quaternion.LookRotation(points[pointCounter].transform.position - _player.transform.position);
            _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, rotation, speed * Time.deltaTime);
            //_player.transform.LookAt(points[pointCounter].transform.position);
        }



        //move to + look at
    }

}
