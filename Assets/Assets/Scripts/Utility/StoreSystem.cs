using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreSystem : MonoBehaviour
{
    private float totalTime;       // how long has the game been running

    public float restockInterval;  // how often should the store stock be exchanged
    private float sinceRestock;    // when was the last restock
    public int stockSize;          // how many items are in stock at one time

    [SerializeField]
    private Upgrade[] riskyItems;  // arrays to hold the possible items the store can stock
    private Upgrade[] safeItems;
    private Upgrade[] stock;       // what items are in the store

    private float safeProb;        // probability of a safe item being stocked

    private LaneSwitcher carInfo;  // get the script that has the car information
    private ScoreSystem scoreInfo; // get the script that has the score information

    // Start is called before the first frame update
    void Start()
    {
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

        // restock the shop
        if(Time.time - sinceRestock >= restockInterval)
        {
            restock();
            sinceRestock = Time.time;
        }
    }

    // handles the input used to purchase from the store
    private void handleInput()
    {
        if (Input.GetButtonDown("Upgrade One"))
            purchase(0);
        if (Input.GetButtonDown("Upgrade Two"))
            purchase(1);
        if (Input.GetButtonDown("Upgrade Three"))
            purchase(2);
    }

    public void restock()
    {
        for(int i = 0; i < stock.Length; ++i)
        {
            // decide whether to stock a safe or risky item
            Upgrade[] selectFrom;
            float prob = Random.Range(0f, 1f);

            if (prob <= safeProb)
                selectFrom = safeItems;
            else
                selectFrom = riskyItems;

            //stock the item
            int indx = Random.Range(0, selectFrom.Length);
            stock[i] = selectFrom[indx];
            Debug.Log("Store slot " + i + ": " + stock[i]);
        }
    }
    
    public void purchase(int toBuyIndex)
    {
        Upgrade toBuy = stock[toBuyIndex];

        if (toBuy != null)
        {
            switch (toBuy.getKey())
            {
                case 0:
                    float speedCost = toBuy.getPrice(totalTime);
                    if (scoreInfo.getMoney() >= speedCost)
                    {
                        Debug.Log("ACTUALLY BUYING SPEED UPGRADE");
                        scoreInfo.spendMoney(speedCost);
                        carInfo.upgradeSpeed(10);
                        stock[toBuyIndex] = null;
                        Debug.Log("Should be null: " + stock[toBuyIndex]);
                    }
                    break;

                case 1:
                    float armorCost = toBuy.getPrice(totalTime);
                    if (scoreInfo.getMoney() >= armorCost)
                    {
                        Debug.Log("Buying Armor");
                        scoreInfo.spendMoney(armorCost);
                        carInfo.upgradeArmor();
                        removeFromItems(riskyItems, toBuy);
                        removeFromStock(toBuy);
                    }
                    break;

                default:
                    break;
            }
        }
        else
            Debug.Log("YOU ALREADY BOUGHT THAT");
    }

    // removes an item toRemove from items
    void removeFromItems(Upgrade[] itemArray, Upgrade toRemove)
    {
        // variable found holds if toRemove is found in items, newItems is all items in items excluding to Remove, and index is used to index newItems
        bool found = false;
        Upgrade[] newItems = new Upgrade[itemArray.Length - 1];
        int index = 0;

        for(int i = 0; i < itemArray.Length; ++i)
        {
            if (itemArray[i] != toRemove)
                newItems[index++] = itemArray[i];
            else
                found = true;
        }

        // prevent errors
        if (!found)
            return;

        // reassign items to newItems
        itemArray = newItems;

        Debug.Log("Redisplaying items");
        for(int i = 0; i < itemArray.Length; ++i)
        {
            Debug.Log(itemArray[i]);
        }
        return;
    }

    void removeFromStock(Upgrade toRemove)
    {
        for(int i = 0; i < stock.Length; ++i)
        {
            if (stock[i] == toRemove)
                stock[i] = null;
        }
    }
}

// used CS 2 slides