using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static NotificationManager;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField] int _maxStamina = 10;
    [SerializeField] float _timeToRecharge = 10f;
    int _currentStamina;

    DateTime _nextStaminaTime;
    DateTime _lastStaminaTime;

    bool recharging;

    [SerializeField] TextMeshProUGUI _staminaText = null;
    [SerializeField] TextMeshProUGUI _timerText = null;

    [SerializeField] string _titleNotif = "Full Stamina";
    [SerializeField] string _textNotif = "Full Stamina, vuelve a jugar";
    [SerializeField] IconSelecter _smallIcon = IconSelecter.icon_reminder;
    [SerializeField] IconSelecter _largeIcon = IconSelecter.icon_reminderBig;
    [SerializeField] 
    TimeSpan timer;
    int id;

    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        StartCoroutine(RechargeStamina());
        if(_currentStamina<_maxStamina)
        {
            timer = _nextStaminaTime - DateTime.Now;
            id = Instance.DisplayNotification(_titleNotif, _textNotif, _smallIcon, _largeIcon,
                AddDuration(DateTime.Now, ((_maxStamina-_currentStamina+1)*_timeToRecharge)+1+(float)timer.TotalSeconds));
        }
    }


    IEnumerator RechargeStamina()
    {
        UpdateTimer();
        UpdateStamina();
        recharging = true;

        while(_currentStamina<_maxStamina)
        {
            DateTime current = DateTime.Now;
            DateTime nexTime = _nextStaminaTime;
            // bool chequea se recargo stamina

            bool addingStamina = false;
            while(current > nexTime)
            {
                if (_currentStamina >= _maxStamina) break;
                _currentStamina += 1;
                addingStamina = true;
                UpdateStamina();

                // predecir prox ves que se va a recargar la stamina
                DateTime timeToAdd = nexTime;
                // chequear si el usuario cerro la app

                if(_lastStaminaTime>nexTime)
                {
                    timeToAdd = _lastStaminaTime;
                }
                nexTime = AddDuration(timeToAdd, _timeToRecharge);

            }    
            // si se recargo stamina
            if(addingStamina)
            {
                _nextStaminaTime = nexTime;
                _lastStaminaTime = DateTime.Now;
            }

            //update ui 
            SaveInformation();
            UpdateStamina();
            UpdateTimer();

            yield return new WaitForEndOfFrame();
        }
       // NotificationManager.Instance.CancelNotification(id);

        recharging = false;
    }

    

    private DateTime AddDuration(DateTime timeToAdd, float timeToRecharge)
    {
        return timeToAdd.AddSeconds(timeToRecharge);
        //return timeToAdd.AddMinutes(timeToRecharge);

    }

    public bool HasEnoughStamina(int stamina)
    {
        return _currentStamina - stamina >= 0;
    }

    public void UseStamina( int staminaToUse)
    {
        if(_currentStamina - staminaToUse >= 0)
        {
            //jugar nivel
            _currentStamina -= staminaToUse;
            UpdateStamina();
            NotificationManager.Instance.CancelNotification(id);
            id = Instance.DisplayNotification(_titleNotif, _textNotif, _smallIcon, _largeIcon,
               AddDuration(DateTime.Now, ((_maxStamina - _currentStamina + 1) * _timeToRecharge) + 1 + (float)timer.TotalSeconds));
            if (!recharging)
            {
                // setear next stamina time y comenzar recarga
                _nextStaminaTime = AddDuration(DateTime.Now, _timeToRecharge);
                StartCoroutine(RechargeStamina());
            }
        }
        else
        {
            Debug.Log("Sin stamina suficiente");
        }
    }


    private void UpdateStamina()
    {
        _staminaText.text = $"{_currentStamina}/{_maxStamina}";
        //_staminaText.text = _currentStamina.ToString("");
    }

    private void UpdateTimer()
    {
        if (_currentStamina >= _maxStamina)
        {
            _timerText.text = "Full stamina";
            return;

        }
        //estructura que de un intervalo de tiempo 
        TimeSpan timer = _nextStaminaTime - DateTime.Now;

        //formato ceros para representar horario como su fuese reloj

        _timerText.text = timer.Minutes.ToString("00") + ":" + timer.Seconds.ToString("00");


    }


    void SaveInformation()
    {
       
        //current stamina
        //next Stamina time
        //last satimana time

        PlayerPrefs.SetInt(SaveData.currentStaminaKey,_currentStamina);
        PlayerPrefs.SetString(SaveData.nextStaminaTimeKey, _nextStaminaTime.ToString());
        PlayerPrefs.SetString(SaveData.lastStaminaTimeKey, _lastStaminaTime.ToString());


    }

    void LoadData()
    {
        _currentStamina = 7;
        //_currentStamina = PlayerPrefs.GetInt(SaveData.currentStaminaKey, _maxStamina);

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

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveInformation();
    }

    private void OnApplicationQuit()
    {
        SaveInformation();
    }

    private void OnDisable()
    {
        SaveInformation();
    }



}
