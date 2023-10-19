using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class PlayerData : MonoBehaviour
{
    [SerializeField] SaveData saveData = new SaveData(); 


    public void SaveGame()
    {
        
        saveData.stamina = GameManager.Instance.stamina;
        saveData.currency = GameManager.Instance.currency;
        saveData.level = GameManager.Instance.level;

        string json = JsonUtility.ToJson(saveData, true); //Crear el string a json                                                    
        PlayerPrefs.SetString("Data",json);
        Debug.Log("save: "+ json);
    }

    public void LoadGame()
    {

        //if (PlayerPrefs.HasKey("Data"))
        
            //string myJson = "load:{\"stamina\":100.0,\"currency\":0.0,\"level\":0}";
            string json = PlayerPrefs.GetString("Data");
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            GameManager.Instance.currency = data.currency;
            GameManager.Instance.stamina = data.stamina;
            GameManager.Instance.level = data.level;          
            
            Debug.Log("load: " + json);


        
       

    }

   

    public void SetCurrency(float actualCurrency)
    {
        saveData.currency = actualCurrency;
    }

}
