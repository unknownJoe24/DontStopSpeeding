using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerUpgrades : MonoBehaviour
{
    //public static ArrayList playerUpgrades;

    private float videoProb;
   // private Upgrade[] tmp;
    private bool al;
    public float timer;
    public float tmpProb;
    public bool videoIsPlaying;
    public GameObject alPanels; 

    [SerializeField]
    // StoreSystem system;
    
     public GameObject alUI;
  //  StreamVideo video;

    // Start is called before the first frame update
    void Start()
    {
        //playerUpgrades = new ArrayList();
       // tmp = new Upgrade[system.stockSize];
        al = true;
        videoProb = 0.9f;
    }

    // Update is called once per frame
    void Update()
    {
        //check store items and add to tmp array
        //once purchased add tmp array item to dynamic array
       /* for(int i = 0; i < system.stockSize; i++)
        {
            if(system.stock[i] != null)
            {
                tmp[i] = system.stock[i];
            } else if (system.stock[i] == null && tmp[i] != null)
            {
                playerUpgrades.Add(tmp[i]);
                if(tmp[i].getKey() == 9)
                {
                    al = true;
                    print("al enabled");
                }
                tmp[i] = null;
                print("Item added to list");
            }
            
        }*/

        //play al video
        if(al)
        {
            VideoPlayer AlVideo = alUI.GetComponentInChildren<VideoPlayer>();

            //print("timer " + timer);
            timer += Time.deltaTime;
            if(timer >= 15.0f)
            {
                alUI.transform.localPosition = new Vector3(Random.Range(-Screen.width, Screen.width), Random.Range(-Screen.height, Screen.height), 0);
                timer = 0;
                if(!AlVideo.isPlaying)
                {
                    tmpProb = Random.Range(0.0f, 1.0f);
                }
                
                print("tmpProb" + tmpProb);
                if (tmpProb <= videoProb && !AlVideo.isPlaying)
                {
                    StartCoroutine(AlVideo.GetComponent<StreamVideo>().PlayVideo());

                }

            }

        }

        
    }
}
