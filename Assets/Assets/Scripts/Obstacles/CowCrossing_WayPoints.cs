using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowCrossing_WayPoints : MonoBehaviour
{

    [SerializeField] private Transform point1, point2;

    //public GameObject group; 
    private float _speed = 1.0f;
    private bool _switch = false;
    public float speed = 1.0f; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if(_switch == false)
        {
            //print("Moving Towards point2");
            gameObject.transform.position = Vector3.MoveTowards(transform.position, point2.position, speed * Time.deltaTime);
        }

        else
        {
            //print("Moving Towards point1");
            gameObject.transform.position = Vector3.MoveTowards(transform.position, point1.position, speed * Time.deltaTime);
        }

        if(transform.position == point2.position)
        {
            //moving towards position 1
            _switch = true;
            //group.transform.localScale = new Vector3(-1, 1, 1); 
        }
        else if (transform.position == point1.position)
        {

            //moving towards position 2
           // group.transform.localScale = new Vector3(-1, 1, 1);
            _switch = false; 
        }
    }
}
