using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public Fighter fighter;  // Aseg�rate de que esto se asigne correctamente en el Inspector

    public TextMeshProUGUI actualDefense;
    public TextMeshProUGUI actualAttack;

    private void Start()
    {
        if (fighter == null)
        {
            Debug.LogError("Fighter no est� asignado en StatsManager.");
            return;
        }

        UpdateUI();
    }

    void Update()
    {
        if (fighter == null)
        {
            Debug.LogError("Fighter no est� asignado en StatsManager.");
            return;
        }

        this.SetDefense(fighter.GetCurrentStats().deffense);
        this.SetAttack(fighter.GetCurrentStats().attack);
    }

    public void UpdateUI()
    {
        if (fighter == null)
        {
            Debug.LogError("Fighter no est� asignado en StatsManager.");
            return;
        }

        this.SetDefense(fighter.GetCurrentStats().deffense);
        this.SetAttack(fighter.GetCurrentStats().attack);
        Debug.Log("se recibi� las estad�sticas de este player: " + fighter);
    }

    public void SetDefense(float deffense)
    {
        if (actualDefense == null)
            return;

        actualDefense.text = deffense.ToString();

        if (deffense >= 80)
        {
            actualDefense.color = Color.yellow;
        }

        if (deffense <= 20)
        {
            actualDefense.color = Color.red;
        }
    }

    public void SetAttack(float attack)
    {
        if (actualAttack == null)
            return;

        actualAttack.text = attack.ToString();

        if (attack >= 80)
        {
            actualAttack.color = Color.yellow;
        }

        if (attack <= 20)
        {
            actualAttack.color = Color.red;
        }
    }
}
