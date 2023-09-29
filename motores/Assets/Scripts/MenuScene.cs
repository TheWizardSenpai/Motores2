using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuScene : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float fadeInSpeed = 0.5f;
    public RectTransform menuContainer;
    public Transform levelPanel;
    public Transform colorPanel;
    public Transform settingPanel;
    public GameObject currency;
    public GameObject stamina;
    public GameObject confirmationPanel;
    public GameObject tutorialPanel;
    public GameObject closeTutorialButton;
    public Button[] levelsButtons;
    int currentIndex = 0;
    public TextMeshProUGUI textInstrucions;
    public List<String> instructionList =new List<String>();


    public bool isFadeOut;



    public TextMeshProUGUI colorBuySetText;  

    private int[] colorCost = new int[] { 0,5,5,5,10,12,12,2,5,10};
   
    private int selectedColorIndex;
 


    private Vector3 desiredMenuPosition;
    IEnumerator Start()
    {
        confirmationPanel.SetActive(false);
        tutorialPanel.SetActive(false);

        fadeGroup = FindObjectOfType<CanvasGroup>();
        fadeGroup.alpha = 1;
        yield return new WaitForSeconds(2.5f);              
        isFadeOut = true;
        //add buttons to shop
        InitShop();
        //add buttons to levels
        InitLevel();
        

    }

    private void Update()
    {
        if(isFadeOut)
        fadeGroup.alpha -= Time.deltaTime * fadeInSpeed;
        // Menu navigation
        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D, desiredMenuPosition, 0.1f);
    }
    private void InitShop()
    {
        if(colorPanel== null)
        {
            Debug.Log("no asign");
        }

        int i = 0;
        foreach (Transform t in colorPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnColorSelect(currentIndex));
            i++;
        }

        i = 0; //reset index
       

    }

    private void InitLevel()
    {
        if (levelPanel == null)
        {
            Debug.Log("no asign");
        }

        int i = 0;
        foreach (Transform t in levelPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnLevelSelect(currentIndex));
            i++;
        }
      
        
    }

    private void NavigateTo(int menuIndex)
    {
        switch (menuIndex)
        {


            //0 && default case = Main Menu
            default:
            case 0:
                desiredMenuPosition = Vector3.zero;
                break;
            //1 Play Menu
            case 1:
                desiredMenuPosition = Vector3.right * 1920;
                break;
            //2 Shop Menu
            case 2:
                desiredMenuPosition = Vector3.left * 1920;
                break;
            case 3:
                desiredMenuPosition = Vector3.up * 1080;
                break;


        }

        if(menuIndex==0||menuIndex==1)
        {
            currency.SetActive(true);
            stamina.SetActive(true);

        }
        else
        {
            currency.SetActive(false);
            stamina.SetActive(false);
        }
    }

    private void SetColor(int index)
    {
        //change the color on the player model

        //change color buyset button
        colorBuySetText.text = "Current";
    }

    

    //buttons

    public void OnPlayClick()
    {
        NavigateTo(1);
        Debug.Log("Play");
    }

    public void OnSettingClick()
    {
        NavigateTo(3);
        Debug.Log("Settings");
    }

    public void OnShopClick()
    {
        NavigateTo(2);
        Debug.Log("Shop");
    }

    public void OnBackClick()
    {
        NavigateTo(0);
        Debug.Log("Back");
    }

   
    private void OnColorSelect(int currentIndex)
    {
        Debug.Log("Color" + currentIndex);

        //set the selected color
        selectedColorIndex = currentIndex;
        //change the content of the buyset button, depending on the state of the color
        if(SaveManager.Instance.IsColorOwned(currentIndex))
        {
            //color is owned
            colorBuySetText.text = "select";
        }
        else
        {
            //color is not owned
            colorBuySetText.text = "Buy:" + colorCost[currentIndex].ToString();
        }

    }

    private void OnLevelSelect(int currentIndex)
    {
        Debug.Log("Level" + currentIndex);
    }
    public void OnColorBuySet()
    {
        Debug.Log("BuyColor");

        //is the selected color owned
        if(SaveManager.Instance.IsColorOwned(selectedColorIndex))
        {
            //set the color
            SetColor(selectedColorIndex);
        }
        else
        {
            //attempt to buy the color
            if(SaveManager.Instance.BuyColor(selectedColorIndex,colorCost[selectedColorIndex]))
            {
                //success
                SetColor(selectedColorIndex);
            }
            else
            {
                // do not have enough gold
                Debug.Log("not enough gold");
            }
        }
    }

    public void EraseDataButton()
    {
        confirmationPanel.SetActive(true);
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        confirmationPanel.SetActive(false);
    }

    public void CancelDeleteData()
    {
        confirmationPanel.SetActive(false);

    }

    public void Tutorial()
    {
        tutorialPanel.SetActive(true);
        StartCoroutine(ChangeInstructions());
    }
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);

    }

    IEnumerator ChangeInstructions()
    {
        while(true)
        {
            textInstrucions.text = instructionList[currentIndex];
            yield return new WaitForSeconds(4);
            currentIndex = (currentIndex + 1) % instructionList.Count;
        }
        


    }


}