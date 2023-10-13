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
        
        saveData.stamina = 0.5f;
        saveData.currency = 1000f;
        saveData.level = 1;

        string json = JsonUtility.ToJson(saveData, true); //Crear el string a json                                                    
        PlayerPrefs.SetString("Data",json);
        Debug.Log("save: "+ json);
    }

    public void LoadGame()
    {

        if (PlayerPrefs.HasKey("Data"))
        {
            string json = PlayerPrefs.GetString("Data");
            JsonUtility.FromJson<SaveData>(json);
            CurrentData();
            Debug.Log("load: " + json);

        }
        else
        {
            Debug.Log("No Key");
        }

    }

    private void CurrentData()
    {
        stamina = saveData.stamina;
        currency = saveData.currency;
        level = saveData.level;
    }

}
