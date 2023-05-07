using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowCrossing_WayPoints : MonoBehaviour
{

    public AudioClip[] cowMoos;

    [SerializeField] private Transform point1, point2;

    //public GameObject group; 
    //private float _speed = 1.0f;
    public bool move = true;
    private bool _switch = false;
    public float speed = 1.0f; 

    private void FixedUpdate()
    {
        if (move)
        {
            if (_switch == false)
            {
                gameObject.transform.position = Vector3.MoveTowards(transform.position, point2.position, speed * Time.deltaTime);
            }
            else
            {
                gameObject.transform.position = Vector3.MoveTowards(transform.position, point1.position, speed * Time.deltaTime);
            }

            if (transform.position == point2.position)
            {
                //moving towards position 1
                _switch = true;
            }
            else if (transform.position == point1.position)
            {
                //moving towards position 2
                _switch = false;
            }
        }
    }
}
