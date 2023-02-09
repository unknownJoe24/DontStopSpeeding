using System.Collections;
using UnityEngine;

public class GasSystem : MonoBehaviour
{
    public float gasLevel = 15.0f;
    public float decreaseRate = 1.0f; // 1 gallon per 20 seconds
    public float maxGasLevel = 20.0f;
    public float purchaseAmount = 5.0f;
    private float lastGasUpdateTime = 0.0f;
    public bool inputDisabled = false;
    private void Update()
    {
        if (inputDisabled)
        {
            return;
        }

        if (Time.time - lastGasUpdateTime >= decreaseRate)
        {
            gasLevel -= 1.0f;
            lastGasUpdateTime = Time.time;
            Debug.Log("Gas level: " + gasLevel);

            if (gasLevel <= 0.0f)
            {
                inputDisabled = true;
                Debug.Log("You lose");
            }
        }

        if (Input.GetButtonDown("Fuel Car"))
        {
            gasLevel += purchaseAmount;
            gasLevel = Mathf.Clamp(gasLevel, 0.0f, maxGasLevel);
            Debug.Log("Purchased gas. Gas level: " + gasLevel);
        }
    }
}