using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StoreDisplay : MonoBehaviour
{
    public Upgrade upgrade; // Upgrade to be displayed

    public TMP_Text displayDesc;
    public TMP_Text displayPrice;
    public Image displayIcon;
    public Sprite defaultImg;

    [SerializeField]
    StoreSystem system;

    [SerializeField]
    GameObject Item;

    private int x;
    
    // Start is called before the first frame update
    void Start()
    {
        displayIcon.sprite = defaultImg;
    }

    // Update is called once per frame
    void Update()
    {
       if (Item.name == "Item 1")
       {
            x = 0;
            // add upgrade selection from StoreSystem
            upgrade = system.stock[x];
            if (system.stock[x] == null)
            {
                displayDesc.text = " ";
                displayPrice.text = " ";
                displayIcon.sprite = defaultImg;
            } else {
                displayDesc.text = upgrade.getDesc();
                displayPrice.text = upgrade.getPrice().ToString();
                displayIcon.sprite = upgrade.getIcon();
            }
       } else if (Item.name == "Item 2")
       {
            x = 1;
            upgrade = system.stock[x];
            if (system.stock[x] == null)
            {
                displayDesc.text = " ";
                displayPrice.text = " ";
                displayIcon.sprite = defaultImg;
            } else {
                displayDesc.text = upgrade.getDesc();
                displayPrice.text = upgrade.getPrice().ToString();
                displayIcon.sprite = upgrade.getIcon();
            }
       } else {
            x = 2;
            upgrade = system.stock[x];
            if (system.stock[x] == null)
            {
                displayDesc.text = " ";
                displayPrice.text = " ";
                displayIcon.sprite = defaultImg;
            } else {
                displayDesc.text = upgrade.getDesc();
                displayPrice.text = upgrade.getPrice().ToString();
                displayIcon.sprite = upgrade.getIcon();
            }
       }
    }
}