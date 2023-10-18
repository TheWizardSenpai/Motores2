
using UnityEngine;
//TP2 GUSTAVO TORRES
public class StatusModSkill : Skill
{
    [Header("Status mod skill")]
    public string message;
    protected StatusMod mod;
    

    protected override void OnRun()
    {
        if (this.mod == null)
        {
            this.mod = this.GetComponent<StatusMod>();
        }


        LogPanel.Write(this.message.Replace("{receiver}", receiver.idName));

        receiver.statusMods.Add(this.mod);
        
 
    }

}