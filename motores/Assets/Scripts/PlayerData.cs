using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class PlayerData : MonoBehaviour
{
    [SerializeField] SaveData saveData = new SaveData();
    public float stamina;
    public float currency;
    public int level;

    

    public void SaveGame()
    {
        
        saveData.stamina = stamina;
        saveData.currency =currency;
        saveData.level = level;

        string json = JsonUtility.ToJson(saveData, true); //Crear el string a json                                                    
        PlayerPrefs.SetString("Data",json);
        Debug.Log("save: "+ json);
    }

    public void LoadGame()
    {

        if (PlayerPrefs.HasKey("Data"))
        {
            string json = PlayerPrefs.GetString("Data");
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            currency = data.currency;
            stamina = data.stamina;
            level = data.level;          
            
            Debug.Log("load: " + json);


        }
        else
        {
            Debug.Log("No Key");
        }

    }

   

    public void SetCurrency(float actualCurrency)
    {
        saveData.currency = actualCurrency;
    }

}
