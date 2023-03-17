using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    // variables for upgrade information
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private string desc;

    // variables used for determining the price
    [SerializeField]
    private float basePrice;
    [SerializeField]
    private float priceMult;
    [SerializeField]
    private float multTime;

    [SerializeField]
    private int key;


    // accessors
    public Sprite getIcon() { return icon; }

    public string getDesc() { return desc; }

    public float getPrice(float time)
    {
        return basePrice + priceMult * Mathf.Floor(time / multTime);
    }

    public int getKey() { return key; }
}
