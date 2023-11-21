using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//TP2 GUSTAVO TORRES/FACUNDO FERREIRO
public class StatsManager : MonoBehaviour
{
    public Fighter fighter;

    public TextMeshProUGUI actualDefense;
    public TextMeshProUGUI actualAttack;

    private void Start()
    {
        UpdateUI();
    }
    void Update()
    {
        //UpdateUI();
        //Debug.Log("Figther" + fighter.stats.attack);
    }

    public void UpdateUI()
    {
        this.SetDefense(fighter.GetCurrentStats().deffense);
        this.SetAttack(fighter.GetCurrentStats().attack);
        Debug.Log("se recibio las estadisticas de estos player" + fighter);
    }

    // Update is called once per frame
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
