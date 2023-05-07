using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public static ArrayList playerUpgrades;

    private float videoProb;
    private Upgrade[] tmp;
    private bool al;
    private float timer;
    
    [SerializeField]
    StoreSystem system;
    [SerializeField]
    StreamVideo video;

    // Start is called before the first frame update
    void Start()
    {
        playerUpgrades = new ArrayList();
        tmp = new Upgrade[system.stockSize];
        al = false;
        videoProb = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        //check store items and add to tmp array
        //once purchased add tmp array item to dynamic array
        for(int i = 0; i < system.stockSize; i++)
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
            
        }

        //play al video
        if(al)
        {
            timer += Time.deltaTime;
            if(timer > 10.0f)
            {
                float tmpProb = Random.Range(0.0f, 1.0f);
                print(tmpProb);
                if (tmpProb <= videoProb)
                {
                    video.PlayVideo();
                }
                timer = 0;
            }
            
        }
    }
}
