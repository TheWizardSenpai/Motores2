using UnityEngine;
//TP2 FACUNDO FERREIRO
public enum HealthModType
{
    STAT_BASED, FIXED, PERCENTAGE
}

public class HealthModSkill : Skill
{
    [Header("Health Mod")]
    public float amount;


    public HealthModType modType;

    [Range(0f, 1f)]
    public float critChance = 0;

    protected override void OnRun()
    {
        float amount = this.GetModification();

        float dice = Random.Range(0f, 1f);


        if (dice <= this.critChance)
        {
            amount *= 2f;
            LogPanel.Write("Critical hit!");
            LogPanel.Write("Hit for " + (Mathf.Abs(amount)).ToString("f0") + (" to " + receiver.idName));
        }

        else
        {
            LogPanel.Write("Hit for " + (Mathf.Abs(amount)).ToString("f0") + (" to " + receiver.idName));
        }


        receiver.ModifyHealth(((int)amount));
    }
    public float GetModification()
    {
        switch (this.modType)
        {
            case HealthModType.STAT_BASED:
                Stats emitterStats = this.emitter.GetCurrentStats();
                Stats receiverStats = receiver.GetCurrentStats();

                // Fórmula: https://bulbapedia.bulbagarden.net/wiki/Damage
                float rawDamage = (((2 * emitterStats.level) / 5) + 2) * this.amount * (emitterStats.attack / receiverStats.deffense);

                return (rawDamage / 50) + 2;
            case HealthModType.FIXED:
                return this.amount;
            case HealthModType.PERCENTAGE:
                Stats rStats = receiver.GetCurrentStats();

                return rStats.maxHealth * this.amount;
        }

        throw new System.InvalidOperationException("HealthModSkill::GetDamage. Unreachable!");
    }
    
}