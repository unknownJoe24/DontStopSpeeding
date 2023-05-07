using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Speedometer : MonoBehaviour
{
    
    private TMP_Text quarterSpeedText;
    private TMP_Text halfSpeedText;
    private TMP_Text threeQSpeedText;
    private TMP_Text maxSpeedText;

    private LaneSwitcher carInfo;

    public float CURRENT_SPEED;
    public float QUARTER_SPEED;
    public float HALF_SPEED;
    public float THREEQ_SPEED;
    public float MAX_SPEED;

    public float minSpeedAngle;
    public float maxSpeedAngle;

    public Transform maxSpeedTransform;

    [Header("UI")]
    //public Text speedLabel;
    public Transform arrow;

    private void Awake()
    {
        carInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<LaneSwitcher>();

        CURRENT_SPEED = carInfo.Speed;
        MAX_SPEED = carInfo.getGearMax();

        quarterSpeedText = GameObject.Find("quarterSpeedText").GetComponent<TMP_Text>();
        halfSpeedText = GameObject.Find("halfSpeedText").GetComponent<TMP_Text>();
        threeQSpeedText = GameObject.Find("threeQSpeedText").GetComponent<TMP_Text>();
        maxSpeedText = GameObject.Find("MaxSpeed").GetComponent<TMP_Text>();

        //maxSpeedTransform.Find("MaxSpeed").GetComponent<Text>().text = Mathf.RoundToInt(MAX_SPEED).ToString();
    }

    /*
    private void CreateSpeedLabels()
    {
        int labelAmount = 10;
        float angleSize = minSpeedAngle - maxSpeedAngle;

        for (int i = 0; i <= labelAmount; i++)
        {
            Transform speedLabelTransform = Instantiate(SpeedLabelTemplateTransform, transform);
            float labelSpeedNormalized = (float)i / labelAmount;
            float speedLabelAngle = minSpeedAngle - labelSpeedNormalized * angleSize;
            speedLabelTransform.eulerAngles = new Vector3(0, 0, speedLabelAngle);
            speedLabelTransform.Find("SpeedText").GetComponent<Text>().text = Mathf.RoundToInt(labelSpeedNormalized * MAX_SPEED).ToString();
            speedLabelTransform.gameObject.SetActive(true);
        }
    }
    */
    private void Update()
    {
        MAX_SPEED = carInfo.getGearMax();
        CURRENT_SPEED = carInfo.Speed;

        QUARTER_SPEED = Mathf.RoundToInt(MAX_SPEED / 4);
        HALF_SPEED = Mathf.RoundToInt(MAX_SPEED / 2);
        THREEQ_SPEED = Mathf.RoundToInt(MAX_SPEED * 0.75f);

        quarterSpeedText.text = GameObject.Find("quarterSpeedText").GetComponent<TMP_Text>().text = Mathf.RoundToInt(QUARTER_SPEED).ToString();
        halfSpeedText.text = GameObject.Find("halfSpeedText").GetComponent<TMP_Text>().text = Mathf.RoundToInt(HALF_SPEED).ToString();
        threeQSpeedText.text = GameObject.Find("threeQSpeedText").GetComponent<TMP_Text>().text = Mathf.RoundToInt(THREEQ_SPEED).ToString();
        maxSpeedText.text = GameObject.Find("MaxSpeed").GetComponent<TMP_Text>().text = Mathf.RoundToInt(MAX_SPEED).ToString();

        arrow.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minSpeedAngle, maxSpeedAngle, CURRENT_SPEED / MAX_SPEED));

    }
    
    private float GetSpeedRotation()
    {
        float totalAngleSize = minSpeedAngle - maxSpeedAngle;

        float speedNormalized = CURRENT_SPEED / MAX_SPEED;

        return minSpeedAngle - speedNormalized * totalAngleSize;
    }
    
}
    


