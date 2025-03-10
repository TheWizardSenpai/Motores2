using UnityEngine;
using System.Collections.Generic;

public abstract class Fighter : MonoBehaviour
{
    public Team team;

    public string idName;
    public StatusPanel statusPanel;

    public CombatManager combatManager;

    public List<StatusMod> statusMods;

    protected Stats stats;
    public Stats GetStats()
    {
        return stats;
    }
    protected Skill[] skills;
    public Animator animator;
    public StatusCondition statusCondition;
    [SerializeField]
    public Transform DamagePivot;
    public bool isAlive
    {
        get => this.stats.health > 0;
    }

    protected virtual void Start()
    {
        this.statusPanel.SetStats(this.idName, this.stats);
        this.skills = this.GetComponentsInChildren<Skill>();

        this.statusMods = new List<StatusMod>();
    }

    protected void AutoConfigureSkillTargeting(Skill skill)
    {
        skill.SetEmitter(this);

        switch (skill.targeting)
        {
            case SkillTargeting.AUTO:
                skill.AddReceiver(this);
                break;
            case SkillTargeting.ALL_ALLIES:
                Fighter[] allies = this.combatManager.GetAllyTeam();
                foreach (var receiver in allies)
                {
                    skill.AddReceiver(receiver);
                }

                break;
            case SkillTargeting.ALL_OPPONENTS:
                Fighter[] enemies = this.combatManager.GetOpposingTeam();
                foreach (var receiver in enemies)
                {
                    skill.AddReceiver(receiver);
                }
                break;

            case SkillTargeting.SINGLE_ALLY:
            case SkillTargeting.SINGLE_OPPONENT:
                throw new System.InvalidOperationException("Unimplemented! This skill needs manual targeting.");
        }
    }

    protected Fighter[] GetSkillTargets(Skill skill)
    {
        switch (skill.targeting)
        {
            case SkillTargeting.AUTO:
            case SkillTargeting.ALL_ALLIES:
            case SkillTargeting.ALL_OPPONENTS:
                throw new System.InvalidOperationException("Unimplemented! This skill doesn't need manual targeting.");

            case SkillTargeting.SINGLE_ALLY:
                return this.combatManager.GetAllyTeam();
            case SkillTargeting.SINGLE_OPPONENT:
                return this.combatManager.GetOpposingTeam();
        }

        // Esto no deber�a ejecutarse nunca pero hay que ponerlo para hacer al compilador feliz.
        throw new System.InvalidOperationException("Fighter::GetSkillTargets. Unreachable!");
    }

    protected void Die()
    {
        this.statusPanel.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void ModifyHealth(float amount)
    {
        this.stats.health = Mathf.Clamp(this.stats.health + amount, 0f, this.stats.maxHealth);

        this.stats.health = Mathf.Round(this.stats.health);
        this.statusPanel.SetHealth(this.stats.health, this.stats.maxHealth);

        if (this.isAlive == false)
        {
            animator.Play("Death");
            Invoke("Die", 2f);
        }

        if (amount > 0f)
        {
            this.animator.Play("Heal");
            Invoke("SetIdleAnimation", 2f);
        }
        else
        {
            this.animator.Play("Damages");
            Invoke("SetIdleAnimation", 2f);
        }

        Invoke("IDLE", 1f);
    }

    public Stats GetCurrentStats()
    {
        Stats modedStats = this.stats;

        foreach (var mod in this.statusMods)
        {
            modedStats = mod.Apply(modedStats);
        }

        return modedStats;
    }

    public StatusCondition GetCurrentStatusCondition()
    {
        if (this.statusCondition != null && this.statusCondition.hasExpired)
        {
            Destroy(this.statusCondition.gameObject);
            this.statusCondition = null;
        }

        return this.statusCondition;
    }


    public abstract void InitTurn();

    private void SetIdleAnimation()
    {
        animator.Play("IDLE");
    }
}
