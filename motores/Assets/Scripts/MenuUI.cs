using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Image fillStamina;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private Button[] levelButtons;
    private int currentLevel;
    [SerializeField] private PlayerData playerData;
    [SerializeField] SaveData saveData = new SaveData();


  

    public void RefreshData()
    {        
        fillStamina.fillAmount = playerData.stamina;
        staminaText.text = fillStamina.fillAmount*100f + " %";
        currencyText.text = playerData.currency.ToString();
        currentLevel = playerData.level;
        EnableButtonsLevel();
    }

    private void EnableButtonsLevel()
    {
        
        foreach (Button btn in levelButtons)
        {
            btn.interactable = false;
        }

        for (int i = 0; i < currentLevel; i++)
        {
            levelButtons[i].interactable = true;

        }      

    }

    public void OnClickButtonLoad()
    {
        playerData.LoadGame();
        fillStamina.fillAmount = playerData.stamina;
        staminaText.text = fillStamina.fillAmount * 100f + " %";
        currencyText.text = playerData.currency.ToString();
        currentLevel = playerData.level;
        EnableButtonsLevel();        
    }

    public void OnClickButtonSave()
    {
        playerData.SaveGame();
    }

}
