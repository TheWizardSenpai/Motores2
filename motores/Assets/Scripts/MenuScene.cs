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
    public bool isFirstTime = true;

    //Inventario
    private Inventory inventory = new Inventory();
    Potion.PotionType currentType;
    public Transform parentItemsPanel;
    public GameObject[] itemsPrefab;


    IEnumerator Start()
    {
        menuUI.RefreshData();

        //if (PlayerPrefs.HasKey("Data"))
        playerData.SaveGame();
        confirmationPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        if(GameManager.Instance.level ==0)
        {
            buttonTutorial.SetActive(false);

        }
        else
        {
            buttonTutorial.SetActive(true);

        }

        if(GameManager.Instance.isFirstTime)
        {
            fadeGroup = FindObjectOfType<CanvasGroup>();
            fadeGroup.alpha = 1;
            yield return new WaitForSeconds(2.5f);
            isFadeOut = true;
            GameManager.Instance.isFirstTime = false;

        }
        //add buttons to shop
        InitShop();
        //add buttons to levels
        InitLevel();
        saveData.currency = GameManager.Instance.currency;
        playerData.LoadGame();
        inventory.LoadInventory();
        InstantiatePotions();

    }

    private void Update()
    {
       
            if (isFadeOut)
            {
                fadeGroup.alpha -= Time.deltaTime * fadeInSpeed;
            }    
            
        
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
            b.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text= "$" + pricesList[i].ToString();
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
        Debug.Log(selectedIndex);

        switch (currentIndex)
        {
            case 0:
                currentType = Potion.PotionType.Type1;
                break;
            case 1:
                currentType = Potion.PotionType.Type2;
                break;
            case 2:
                currentType = Potion.PotionType.Type3;
                break;
            case 3:
                currentType = Potion.PotionType.Type4;
                break;
        }
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

    private void OnLevelSelect(int currentIndex) // para despues
    {
        Debug.Log("Level" + currentIndex);
        if(!viewTutorial)
        {
            Tutorial();
            viewTutorial = true;
        }
        else
        {
            GameManager.Instance.stamina = GameManager.Instance.stamina - 0.3f;

            SceneLevel("Motores2");
        }
    }

    public void RechargeStamina()
    {
        GameManager.Instance.stamina = GameManager.Instance.stamina + 0.05f;
        menuUI.RefreshData();
    }

    private void SceneLevel(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }
    public void OnBuy(int quantity = 1)
    {
        float priceItem = pricesList[selectedIndex];
        Debug.Log(saveData.currency);
        if (saveData.currency>priceItem)
        {
            Potion newPotion = new Potion(currentType, quantity);
            inventory.AddPotion(newPotion);

            if (parentItemsPanel.transform.childCount > 0)
            {
                bool found = false;

                foreach (Transform child in parentItemsPanel.transform)
                {
                    if (child.name == selectedIndex.ToString() + "(Clone)")
                    {
                        Debug.Log(selectedIndex.ToString() + "(Clone)");

                        int a = int.Parse(child.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text);
                        int b = a + quantity;

                        child.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = b.ToString();
                        found = true;
                        break;
                    }                    
                }
                if (!found)
                {
                    Instantiate(itemsPrefab[selectedIndex], parentItemsPanel);
                }
            }
            else
            {
                Instantiate(itemsPrefab[selectedIndex], parentItemsPanel);
            }

            float currentMoney = saveData.currency;
            float result = currentMoney - priceItem;
            currencyText.text = result.ToString();
            saveData.currency = result;
            GameManager.Instance.currency = result;
            playerData.SaveGame();
            inventory.SaveInventory();
            Debug.Log("Compra " + currentType);
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    private void InstantiatePotions()
    {
        foreach (Potion potion in inventory.GetPotions())
        {
            if (potion.Quantity > 0)
            {
                int prefabIndex = (int)potion.potionType;
                GameObject potionPrefab = itemsPrefab[prefabIndex];
                GameObject potionInstance = Instantiate(potionPrefab, parentItemsPanel);
                TextMeshProUGUI quantityText = potionInstance.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                quantityText.text = potion.Quantity.ToString();             
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
        GameManager.Instance.currency = 0;
        GameManager.Instance.level = 0;
        GameManager.Instance.stamina = 0;
        menuUI.RefreshData();
        PlayerData.Get().SaveGame();
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
