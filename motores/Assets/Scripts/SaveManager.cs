using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; set; }
    public SaveState state;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();
        Debug.Log(Helper.Serialize<SaveState>(state));
    }

    //save script to the playerPref

    public void Save()
    {
        PlayerPrefs.SetString("save",Helper.Serialize<SaveState>(state));
    }

    //load the previous saved state 
    public void Load()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            state = Helper.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
        }
        else
        {
            state = new SaveState();
            Save();
            Debug.Log("No save file found, creatin a new one");
        }
    }

    //check if the color is owned

    public bool IsColorOwned(int index)
    {
        //check if the bit is set, if so the color is owned
        return (state.colorOwned & (1 << index)) != 0;
    }

    //check if the trail is owned

    public bool IsTrailOwned(int index)
    {
       
        return (state.trailOwned & (1 << index)) != 0;
    }
    //attempt buying a color, return true false
    public bool BuyColor(int index, int cost)
    {
        if(state.gold>=cost)
        {
            //enough money, remove from the current gold stack
            state.gold -= cost;
            UnlockColor(index);
            //save porgress
            Save();
            return true;
        }
        else
        {
            //not enough money, return false

            return false;
        }
    }

    public bool BuyTrail(int index, int cost)
    {
        if (state.gold >= cost)
        {
            //enough money, remove from the current gold stack
            state.gold -= cost;
            UnlockTrail(index);
            //save porgress
            Save();
            return true;
        }
        else
        {
            //not enough money, return false

            return false;
        }
    }


    //unlock the color in the color owned
    public void UnlockColor(int index)
    {
        //togle on the bit at index
        state.colorOwned |= 1 << index;

    }
    //unlock the trail in the trail owned
    public void UnlockTrail(int index)
    {
        //togle on the bit at index
        state.trailOwned |= 1 << index;

    }
    //reset the whole save file
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }
}
