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
        fillStamina.fillAmount = GameManager.Instance.stamina / 100;
        staminaText.text = fillStamina.fillAmount*100f + " %";
        currencyText.text = GameManager.Instance.currency.ToString();
        currentLevel = GameManager.Instance.level;
        EnableButtonsLevel();
    }

    private void EnableButtonsLevel()
    {
        
        foreach (Button btn in levelButtons)
        {
            btn.interactable = false;
        }

        for (int i = 0; i <= currentLevel; i++)
        {
            levelButtons[i].interactable = true;
        }      
    }

    public void OnClickButtonLoad()
    {
        playerData.LoadGame();
        fillStamina.fillAmount = GameManager.Instance.stamina;
        staminaText.text = fillStamina.fillAmount * 100f + " %";
        currencyText.text = GameManager.Instance.currency.ToString();
        currentLevel = GameManager.Instance.level;
        EnableButtonsLevel();        
    }

    public void OnClickButtonSave()
    {
        playerData.SaveGame();
    }

}
