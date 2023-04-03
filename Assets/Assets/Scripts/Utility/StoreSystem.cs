using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreSystem : MonoBehaviour
{
    public float restockInterval;  // how often should the store stock be exchanged
    private float sinceRestock;    // when was the last restock
    public int stockSize;          // how many items are in stock at one time

    public Upgrade[] riskyItems;   // arrays to hold the possible items the store can stock
    public Upgrade[] safeItems;
    public Upgrade[] stock;        // what items are in the store

    private float safeProb;        // probability of a safe item being stocked, intrinsically linked to risky probability

    private float safePriceMult;   // multiplier of the price for safe prices
    private float riskyPriceMult;  // multiplier of the price for risky prices

    private LaneSwitcher carInfo;  // get the script that has the car information
    private ScoreSystem scoreInfo; // get the script that has the score information

    // Start is called before the first frame update
    void Start()
    {

        stock = new Upgrade[stockSize];

        restock();
        sinceRestock = Time.time;

        safeProb = .5f;

        safePriceMult = 1f;
        riskyPriceMult = 1f;

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

            if (prob <= safeProb || riskyItems.Length < 1)
                selectFrom = safeItems;
            else
                selectFrom = riskyItems;

            //stock the item
            int indx = Random.Range(0, selectFrom.Length);
            stock[i] = selectFrom[indx];
            //Debug.Log("Store slot " + i + ": " + stock[i]);
        }
    }
    
    public void purchase(int toBuyIndex)
    {
        Upgrade toBuy = stock[toBuyIndex];

        // only buy the item if it's in stock(not bought previously)
        if (toBuy != null)
        {
            switch (toBuy.getKey())
            {
                // engine upgrade
                case 0:
                    float speedCost = toBuy.getPrice() * safePriceMult;
                    if (scoreInfo.getMoney() >= speedCost)
                    {
                        //Debug.Log("ACTUALLY BUYING SPEED UPGRADE");
                        scoreInfo.spendMoney(speedCost);
                        carInfo.upgradeEngine(10);
                        stock[toBuyIndex] = null;
                    }
                    break;

                // armor upgrade
                case 1:
                    float armorCost = toBuy.getPrice() * riskyPriceMult;
                    if (scoreInfo.getMoney() >= armorCost)
                    {
                        //Debug.Log("Buying Armor");
                        scoreInfo.spendMoney(armorCost);
                        carInfo.upgradeArmor();
                        removeFromRiskyItems(toBuy);
                        removeFromStock(toBuy);
                    }
                    break;

                // tank upgrade
                case 2:
                    float tankCost = toBuy.getPrice() * safePriceMult;
                    if(scoreInfo.getMoney() >= tankCost)
                    {
                        scoreInfo.spendMoney(tankCost);
                        carInfo.upgradeTank(.95f);
                        stock[toBuyIndex] = null;
                    }
                    break;

                // amphibious upgrade
                case 3:
                    float amphibiousCost = toBuy.getPrice() * riskyPriceMult;
                    if(scoreInfo.getMoney() >= amphibiousCost)
                    {
                        scoreInfo.spendMoney(amphibiousCost);
                        carInfo.upgradeAmphibious();
                        removeFromRiskyItems(toBuy);
                        removeFromStock(toBuy);
                    }
                    break;

                // SAFE Rewards upgrade
                case 4:
                    float SAFECost = toBuy.getPrice() * riskyPriceMult;
                    if(scoreInfo.getMoney() >= SAFECost)
                    {
                        scoreInfo.spendMoney(SAFECost);
                        SAFERewards();
                        removeFromRiskyItems(toBuy);
                        removeFromStock(toBuy);
                    }
                    break;

                // RISKY Rewards upgrade
                case 5:
                    float RISKYCost = toBuy.getPrice() * riskyPriceMult;
                    if (scoreInfo.getMoney() >= RISKYCost)
                    {
                        scoreInfo.spendMoney(RISKYCost);
                        RISKYRewards();
                        removeFromRiskyItems(toBuy);
                        removeFromStock(toBuy);
                    }
                    break;

                // Grease Monkey upgrade
                case 6:
                    float GreaseMonkeyCost = toBuy.getPrice() * safePriceMult;
                    if(scoreInfo.getMoney() >= GreaseMonkeyCost)
                    {
                        scoreInfo.spendMoney(GreaseMonkeyCost);
                        // input upgrade func here
                        stock[toBuyIndex] = null;
                    }
                    break;

                // Multiplier upgrade
                case 7:
                    float multiplierCost = toBuy.getPrice() * safePriceMult;
                    if(scoreInfo.getMoney() >= multiplierCost)
                    {
                        scoreInfo.spendMoney(multiplierCost);
                        // input upgrade func here
                        stock[toBuyIndex] = null;
                    }
                    break;

                // Ramp upgrade
                case 8:
                    float rampCost = toBuy.getPrice() * riskyPriceMult;
                    if(scoreInfo.getMoney() >= rampCost)
                    {
                        scoreInfo.spendMoney(rampCost);
                        // input upgrade func here
                        removeFromRiskyItems(toBuy);
                        removeFromStock(toBuy);
                    }
                    break;

                // Al upgrade
                case 9:
                    float AlCost = toBuy.getPrice() * riskyPriceMult;
                    if(scoreInfo.getMoney() >= AlCost)
                    {
                        scoreInfo.spendMoney(AlCost);
                        // input upgrade func here
                        removeFromRiskyItems(toBuy);
                        removeFromStock(toBuy);
                    }
                    break;


                default:
                    break;
            }
        }
        //else
            //Debug.Log("YOU ALREADY BOUGHT THAT");
    }

    // removes an item toRemove from items
    void removeFromRiskyItems(Upgrade toRemove)
    {
        // variable found holds if toRemove is found in items, newItems is all items in items excluding to Remove, and index is used to index newItems
        bool found = false;
        Upgrade[] newItems = new Upgrade[riskyItems.Length - 1];
        int index = 0;

        for(int i = 0; i < riskyItems.Length; ++i)
        {
            if (riskyItems[i] != toRemove)
                newItems[index++] = riskyItems[i];
            else
                found = true;
        }

        // prevent errors
        if (!found)
            return;

        // reassign items to newItems
        riskyItems = newItems;

        //Debug.Log("Redisplaying items");
        for(int i = 0; i < riskyItems.Length; ++i)
        {
            Debug.Log(riskyItems[i]);
        }
        return;
    }

    // 'remove' an item globally from the stock
    void removeFromStock(Upgrade toRemove)
    {
        for(int i = 0; i < stock.Length; ++i)
        {
            if (stock[i] == toRemove)
                stock[i] = null;
        }
    }

    void SAFERewards()
    {
        safeProb *= 1.2f;
        safePriceMult *= .8f;
    }

    void RISKYRewards()
    {
        safeProb *= .8f;
        riskyPriceMult *= .8f;
    }

}

// used CS 2 slides