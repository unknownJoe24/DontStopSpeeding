using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreSystem : MonoBehaviour
{
    public float restockInterval;  // how often should the store stock be exchanged
    private float sinceRestock;    // when was the last restock
    public int stockSize;          // how many items are in stock at one time

    public Upgrade[] riskyItems;   // arrays to hold the possible items the store can stock - should not contain any duplicates
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
        // set the stock size - NOTE: A size != 3 will currently cause issues
        stock = new Upgrade[stockSize];

        // restock the shop
        restock();

        // initialize the probability of safe items being stocked (risky are 1 - safeProb)
        safeProb = .5f;

        // initialize the probabilities for the price multipliers (for R.I.S.K.Y and S.A.F.E. Rewards)
        safePriceMult = 1f;
        riskyPriceMult = 1f;

        // initialize the variables storing the car(player) and score info
        carInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<LaneSwitcher>();
        scoreInfo = GameObject.FindGameObjectWithTag("ScoreHandler").GetComponent<ScoreSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // allow the player to buy upgrades
        handleInput();

        // restock the shop
        if(Time.time - sinceRestock >= restockInterval)
            restock();
    }

    // handles the input used to purchase from the store
    private void handleInput()
    {
        // purchase the upgrade corresponding to the key the user pressed
        if (Input.GetButtonDown("Upgrade One"))
            purchase(0);
        if (Input.GetButtonDown("Upgrade Two"))
            purchase(1);
        if (Input.GetButtonDown("Upgrade Three"))
            purchase(2);
    }
    
    // restocks the shop's inventory
    public void restock()
    {
        // restock each index of the stock
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
        }

        // save the time that the store was restocked
        sinceRestock = Time.time;
    }
    
    public void purchase(int toBuyIndex)
    {
        // the actual item they wish to buy
        Upgrade toBuy = stock[toBuyIndex];

        // only buy the item if it's in stock(not bought previously)
        if (toBuy != null)
        {
            // buy the corresponding upgrade
            switch (toBuy.getKey())
            {
                // engine upgrade
                case 0:
                    float speedCost = toBuy.getPrice() * safePriceMult;
                    if (scoreInfo.getMoney() >= speedCost)
                    {
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
                        carInfo.upgradeGreaseMonkey(.95f);
                        stock[toBuyIndex] = null;
                    }
                    break;

                // Multiplier upgrade
                case 7:
                    float multiplierCost = toBuy.getPrice() * safePriceMult;
                    if(scoreInfo.getMoney() >= multiplierCost)
                    {
                        scoreInfo.spendMoney(multiplierCost);
                        upgradeScoreMult();
                        stock[toBuyIndex] = null;
                    }
                    break;

                // Ramp upgrade
                case 8:
                    float rampCost = toBuy.getPrice() * riskyPriceMult;
                    if(scoreInfo.getMoney() >= rampCost)
                    {
                        scoreInfo.spendMoney(rampCost);
                        carInfo.upgradeRamp();
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
                        carInfo.upgradeAl();
                        removeFromRiskyItems(toBuy);
                        removeFromStock(toBuy);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    // removes an item toRemove from items
    void removeFromRiskyItems(Upgrade toRemove)
    {
        // variable found holds if toRemove is found in items, newItems is all items in items excluding toRemove, and index is used to index newItems
        bool found = false;
        Upgrade[] newItems = new Upgrade[riskyItems.Length - 1];
        int index = 0;

        // remove the item from riskyItems
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

        return;
    }

    // 'remove' an item globally from the stock
    void removeFromStock(Upgrade toRemove)
    {
        // removes the item from each index of stock
        for(int i = 0; i < stock.Length; ++i)
        {
            if (stock[i] == toRemove)
                stock[i] = null;
        }
    }

    // S.A.F.E. Rewards implementation
    void SAFERewards()
    {
        safeProb *= 1.2f;
        safePriceMult *= .8f;
    }

    // R.I.S.K.Y. Rewards implementation
    void RISKYRewards()
    {
        safeProb *= .8f;
        riskyPriceMult *= .8f;
    }

    // Score Multiplier upgrade implementation
    void upgradeScoreMult()
    {
        // is this what we wanted?
        scoreInfo.storeScoreMult *= 1.1f;
    }

}

// used CS 2 slides