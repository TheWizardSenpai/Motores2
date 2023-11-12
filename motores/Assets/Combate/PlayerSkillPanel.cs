
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSkillPanel : MonoBehaviour
{
    public GameObject[] skillButtons;
    public Text[] skillButtonLabels;
    public GameObject skillPanel;
    public TextMeshProUGUI levelText;

    private void Awake()
    {
        this.Hide();

        foreach (var btn in this.skillButtons)
        {
            btn.SetActive(false);
        }

    }
    private void Start()
    {
        levelText.text = "Level " + (GameManager.Instance.level+1).ToString();
    }
    public void ConfigureButtons(int index, string skillName)
    {
        this.skillButtons[index].SetActive(true);
        this.skillButtonLabels[index].text = skillName;
    }

    public void Show()
    {
        this.skillPanel.SetActive(true);
    }

    public void Hide()
    {
        this.skillPanel.SetActive(false);
    }
}
