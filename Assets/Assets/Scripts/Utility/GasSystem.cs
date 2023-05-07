using UnityEngine;
using UnityEngine.UI;

public class GasSystem : MonoBehaviour
{
    public float gasLevel = 15.0f;
    public float decreaseRate = 20.0f; // 1 gallon per 20 seconds
    public float maxGasLevel = 15.0f;
    public float purchaseAmount = 5.0f;
    public float purchasePrice = 50.0f;
    public GameObject currScoreHandler; //use private scoreSystem?
    private float lastGasUpdateTime = 0.0f;
    public bool inputDisabled = false;
    public Slider slider;
    public Image fillBar;
    public LaneSwitcher laneSwitcher;

    public void Start()
    {
        laneSwitcher = GetComponent<LaneSwitcher>();
        slider.maxValue = maxGasLevel;
    }
    private void Update()
    {
        if (inputDisabled)
        {
            laneSwitcher.DisableMovement= inputDisabled;
            return;
        }


        if (Time.time - lastGasUpdateTime >= decreaseRate)
        {
            gasLevel -= 1.0f;
            lastGasUpdateTime = Time.time;
            SetGasValue(gasLevel);

            if (gasLevel <= 0.0f)
            {
                gasLevel = 0.0f;
                SetGasValue(gasLevel);
                inputDisabled = true;
            }
        }

        if (Input.GetButtonDown("Fuel Car") && Time.timeScale != 0)
        {
            if (currScoreHandler.GetComponent<ScoreSystem>().spendMoney(purchasePrice))
            {
                gasLevel += purchaseAmount;
                gasLevel = Mathf.Clamp(gasLevel, 0.0f, maxGasLevel);
                SetGasValue(gasLevel);
            }
        }
    }

    public void SetGasValue(float amount)
    {
        slider.value = amount;
        SetGasBarColor(slider.value);
    }

    private void SetGasBarColor(float value)
    {
        switch (value)
        {
            case (> 3.75f) when value <= 7.5:
                fillBar.color = new Color(255, 255, 0, 100);
                break;
            case (<= 3.75f):
                fillBar.color = new Color(255, 0, 0, 100);
                break;
            default:
                fillBar.color = new Color(0, 255, 0, 100);
                break;
        }
    }

}