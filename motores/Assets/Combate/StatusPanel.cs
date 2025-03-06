using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour
{
    public Text nameLabel;
    public Text levelLabel;

    public Image healthBar;
    public Text healthLabel;
    public TextMeshProUGUI nameTextLabel;
    public TextMeshProUGUI actualDefense;
    public TextMeshProUGUI actualAttack;
    public void SetStats(string name, Stats stats)
    {
        if (nameLabel != null)
            this.nameLabel.text = name;

        if (nameTextLabel != null)
        {
            this.nameTextLabel.text = name;
            this.nameTextLabel.fontSize = name.Length > 8 ? 4 : 6;

        }
        if (levelLabel != null)
            this.levelLabel.text = $"N. {stats.level}";
        if (actualAttack != null)
        {
            this.actualAttack.text = $"{stats.attack}";
        }
        if (actualDefense != null)
        {
            this.actualDefense.text = $"{stats.deffense}";
        }


        this.SetHealth(stats.health, stats.maxHealth);

    }
    public void SetHealth(float health, float maxHealth)
    {
        //Matfh convierte el flotante health en numeros enteros para que no salgan decimales
        this.healthLabel.text = $"{Mathf.RoundToInt(health)} / {Mathf.RoundToInt(maxHealth)}";
        float percentage = health / maxHealth;

        this.healthBar.fillAmount = percentage;
        //Si el porcentaje de vida es menor al 33% el color de la vida se vuelve rojo
        if (percentage < 0.33f)
        {
            this.healthBar.color = Color.red;
        }
        
    }
}