using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreSystem : MonoBehaviour
{
    private float totalTime;       // how long has the game been running

    public float restockInterval;  // how often should the store stock be exchanged
    private float sinceRestock;    // when was the last restock
    public int stockSize;          // how many items are in stock at one time
    private int itemNum;           // how many items does the store stock

    [SerializeField]
    private Upgrade[] items;       // array to hold the possible items the store can stock

    /* non-scriptable object method
    enum safeUp { speed = 0 }
    enum riskUp { armor = 1 }
    */

    //private int[] stockKeys;       // keys relating to what items are in stock
    private Upgrade[] stock;

    private LaneSwitcher carInfo;  // get the script that has the car information
    private ScoreSystem scoreInfo; // get the script that has the score information

    // Start is called before the first frame update
    void Start()
    {
        //itemNum = 3;
        itemNum = items.Length;

        //stockKeys = new int[stockSize];
        stock = new Upgrade[stockSize];

        restock();
        sinceRestock = Time.time;

        carInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<LaneSwitcher>();
        scoreInfo = GameObject.FindGameObjectWithTag("ScoreHandler").GetComponent<ScoreSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        handleInput();

        if(Time.time - sinceRestock >= restockInterval)
        {
            restock();
            sinceRestock = Time.time;
        }
    }

    // handles the input used to purchase from the store
    private void handleInput()
    {
        /*
        if (Input.GetButtonDown("Upgrade One"))
            purchase(stockKeys[0]);
        if (Input.GetButtonDown("Upgrade Two"))
            purchase(stockKeys[1]);
        if (Input.GetButtonDown("Upgrade Three"))
            purchase(stockKeys[2]);
        */

        if (Input.GetButtonDown("Upgrade One"))
            purchase(stock[0]);
        if (Input.GetButtonDown("Upgrade Two"))
            purchase(stock[1]);
        if (Input.GetButtonDown("Upgrade Three"))
            purchase(stock[2]);
    }

    public void restock()
    {
        /*
        // populate the store's stock
        for (int i = 0; i < stockSize; ++i)
        {
            stockKeys[i] = Random.Range(0, itemNum - 1);
            Debug.Log("Store slot " + i + ": " + stockKeys[i]);
        }
        */

        for(int i = 0; i < stockSize; ++i)
        {
            int indx = Random.Range(0, itemNum);
            stock[i] = items[indx];
            Debug.Log("Store slot " + i + ": " + stock[i]);
        }
    }
    
    public void purchase(/*int stockKey*/ Upgrade toBuy)
    {
        /*
        switch(stockKey)
        {
            case (int) safeUp.speed:
                float speedCost = (totalTime / 120 + 1) * 1000;
                if(scoreInfo.getMoney() > speedCost)
                {
                    scoreInfo.spendMoney(speedCost);
                    carInfo.upgradeSpeed(20);
                }
                break;

            case (int) riskUp.armor:
                float armorCost = (totalTime / 60 + 1) * 10000;
                if(scoreInfo.getMoney() > armorCost && ! carInfo.getArmor())
                {
                    scoreInfo.spendMoney(armorCost);
                    carInfo.upgradeArmor();
                }
                break;

            default:
                break;
        }
        */

        switch(toBuy.getKey())
        {
            case 0:
                float speedCost = toBuy.getPrice(totalTime);
                if (scoreInfo.getMoney() >= speedCost)
                {
                    scoreInfo.spendMoney(speedCost);
                    carInfo.upgradeSpeed(10);
                }
                break;

            case 1:
                float armorCost = toBuy.getPrice(totalTime);
                if (scoreInfo.getMoney() >= armorCost)
                    carInfo.upgradeArmor();
                break;

            default:
                break;
        }
    }
}