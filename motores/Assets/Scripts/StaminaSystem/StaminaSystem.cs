using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField] int _maxStamina = 10;
    [SerializeField] float _timeToRecharge = 10f;
    int currentStamina;

    DateTime _nextStaminaTime;
    DateTime _lastStaminaTime;

    bool recharging;

    [SerializeField] TextMeshProUGUI _staminaText = null;
    [SerializeField] TextMeshProUGUI _timerText = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SaveInformation()
    {
       
        //current stamina
        //next Stamina time
        //last satimana time

        //PlayerPrefs.SetInt(SaveData.currentStaminaKey,_currentStamina);
        PlayerPrefs.SetString(SaveData.nextStaminaTimeKey, _nextStaminaTime.ToString());
        PlayerPrefs.SetString(SaveData.lastStaminaTimeKey, _lastStaminaTime.ToString());


    }

    void LoadData()
    {
        //_currentStamina = PlayerPrefs.GetInt(SaveData.currentStaminaKey);

        //_nextStaminaTime = DateTime.Parse(PlayerPrefs.GetString(SaveData.nextStaminaTimeKey));
        //_lastStaminaTime = DateTime.Parse(PlayerPrefs.GetString(SaveData.lastStaminaTimeKey));

        _nextStaminaTime = StringToDateTime(PlayerPrefs.GetString(SaveData.nextStaminaTimeKey));
        _lastStaminaTime = StringToDateTime(PlayerPrefs.GetString(SaveData.lastStaminaTimeKey));

    }

    DateTime StringToDateTime(string date)
    {
        if (string.IsNullOrEmpty(date))
            return DateTime.Now; //utcNow
        else
            return DateTime.Parse(date);
    }
}
