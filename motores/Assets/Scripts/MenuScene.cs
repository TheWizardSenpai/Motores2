using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float fadeInSpeed = 0.5f;
    public RectTransform menuContainer;
    public Transform levelPanel;
    public Transform colorPanel;
    public Transform settingPanel;
    public GameObject currencyPanel;
    public GameObject staminaPanel;
    public GameObject confirmationPanel;
    public GameObject tutorialPanel;
    public bool viewTutorial;
    public GameObject buttonTutorial;
    public GameObject closeTutorialButton;
    public Button[] levelsButtons;
    int currentIndex = 0;
    public TextMeshProUGUI textInstrucions;
    public List<String> instructionList =new List<String>();
    public List<float> pricesList = new List<float>();
    public bool isFadeOut;
    public TextMeshProUGUI colorBuySetText;   
    private int selectedIndex;
    [SerializeField] SaveData saveData = new SaveData();
    public TextMeshProUGUI currencyText;
    public PlayerData playerData;
    private Vector3 desiredMenuPosition;
    public MenuUI menuUI;


    private void Awake()
    {
        playerData.LoadGame();
    }
    

    IEnumerator Start()
    {
        confirmationPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        if(playerData.level==0)
        {
            buttonTutorial.SetActive(false);

        }
        else
        {
            buttonTutorial.SetActive(true);

        }
        fadeGroup = FindObjectOfType<CanvasGroup>();
        fadeGroup.alpha = 1;
        yield return new WaitForSeconds(2.5f);              
        isFadeOut = true;
        //add buttons to shop
        InitShop();
        //add buttons to levels
        InitLevel();
        saveData.currency = playerData.currency;
        menuUI.RefreshData();

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
            //b.onClick.AddListener(() => OnColorSelect(currentIndex));
            b.onClick.AddListener(() => OnItemSelect(b, currentIndex));
            b.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text=pricesList[i].ToString();
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

        
        if(menuIndex==0||menuIndex==1 || menuIndex ==2 )
        {
            currencyPanel.SetActive(true);
            staminaPanel.SetActive(true);

        }
        else
        {
            currencyPanel.SetActive(false);
            staminaPanel.SetActive(false);
        }
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

   
    private void OnItemSelect(Button btn, int currentIndex)
    {
        selectedIndex = currentIndex;
        clearColorButtons();
        btn.gameObject.GetComponent<Image>().color = Color.green;
    }

    private void clearColorButtons()
    {
        int i = 0;
        foreach (Transform t in colorPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.gameObject.GetComponent<Image>().color = Color.white;
            i++;
        }
    }

    private void OnLevelSelect(int currentIndex)
    {
        Debug.Log("Level" + currentIndex);
        if(!viewTutorial)
        {
            Tutorial();
            viewTutorial = true;
        }
        else
        {
            SceneLevel("Motores2");
        }
    }

    private void SceneLevel(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }
    public void OnBuy()
    {
        float priceItem = pricesList[selectedIndex];
        Debug.Log(saveData.currency);
        if (saveData.currency>priceItem)
        {
            float currentMoney = saveData.currency;
            float result = currentMoney - priceItem;
            currencyText.text = result.ToString();
            saveData.currency = result;
            playerData.currency = result;
            playerData.SaveGame();
        }
        else
        {
            Debug.Log("Not enough money");
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
