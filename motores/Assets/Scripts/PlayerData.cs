using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class PlayerData : MonoBehaviour
{
    [SerializeField] SaveData saveData = new SaveData();

    private static PlayerData instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static PlayerData Get ()
    {
        return instance;
    }    

    public void SaveGame()
    {        
        saveData.stamina = GameManager.Instance.stamina;
        saveData.currency = GameManager.Instance.currency;
        saveData.level = GameManager.Instance.level;
        saveData.nextStamina = GameManager.Instance.nextStamina;
        saveData.lastStamina = GameManager.Instance.lastStamina;

        string json = JsonUtility.ToJson(saveData, true); //Crear el string a json                                                    
        PlayerPrefs.SetString("Data",json);
        Debug.Log("save: "+ json);
    }

    public void LoadGame()
    {
        string json = PlayerPrefs.GetString("Data");
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        GameManager.Instance.currency = data.currency;
        GameManager.Instance.stamina = data.stamina;
        GameManager.Instance.level = data.level;
        GameManager.Instance.nextStamina= data.nextStamina;
        GameManager.Instance.lastStamina = data.lastStamina;

        Debug.Log("load: " + json);   
    }
      

    public void SetCurrency(float actualCurrency)
    {
        saveData.currency = actualCurrency;
    }

}
