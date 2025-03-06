using UnityEngine;
using System.Collections.Generic;
public class PlayerFighter : Fighter
{
    [Header("UI")]
    public PlayerSkillPanel skillPanel;
    public EnemiesPanel enemiesPanel;
    public int figherIndex;
    private Skill skillToBeExecuted;
    public EnemyDataBase fightersDateBase;


    private int activeAllyIndex;
    
   

    private List<Fighter> allies;
    void Awake()
    {
        var data = fightersDateBase.EnemyDB[figherIndex];
        //_IAEnemySimple = gameObject.GetComponent<IAEnemySimple>();
        //

        if (data.level != 0)
            this.stats = new Stats(data.level, data.maxHealth, data.attack, data.deffense, data.spirit, data.speed);
        else
            this.stats = new Stats(21, 60, 50, 45, 20, 20);

        allies = new List<Fighter>();
        allies.Add(this); // Agregar al jugador actual como el primer aliado activo
        activeAllyIndex = 0; // Establecer el jugador actual como el aliado activo inicialmente

    }

    public override void InitTurn()
    {
        this.skillPanel.ShowForPlayer(this);

        for (int i = 0; i < this.skills.Length; i++)
        {
            this.skillPanel.ConfigureButton(i, this.skills[i].skillName);
        }
    }
    /// ================================================
    /// <summary>
    /// Se llama desde EnemiesPanel.
    /// </summary>
    /// <param name="index"></param>
    public void ExecuteSkill(int index)
    {
        this.skillToBeExecuted = this.skills[index];
        this.skillToBeExecuted.SetEmitter(this);
        animator.Play("Attack");
        if (this.skillToBeExecuted.needsManualTargeting)
        {
            Fighter[] receivers = this.GetSkillTargets(this.skillToBeExecuted);
            this.enemiesPanel.Show(this, receivers);
        }
        else
        {
            this.AutoConfigureSkillTargeting(this.skillToBeExecuted);

            this.combatManager.OnFighterSkill(this.skillToBeExecuted);
            this.skillPanel.Hide();
            Invoke(nameof(ResetAnimation), 0.5f);
        }
    }

    public void SetTargetAndAttack(Fighter enemyFigther)
    {
        this.skillToBeExecuted.AddReceiver(enemyFigther);

        this.combatManager.OnFighterSkill(this.skillToBeExecuted);

        this.skillPanel.Hide();
        this.enemiesPanel.Hide();
    }
    private void ResetAnimation()
    {
        animator.Play("IDLE"); // Cambia la animaci?n a Idle
    }
    public PlayerFighter GetSkillPanel(PlayerSkillPanel newSkillPanel, StatusPanel newStatusPanel, EnemiesPanel newEnemiesPanel)
    {
     skillPanel = newSkillPanel;
     statusPanel = newStatusPanel;
    enemiesPanel = newEnemiesPanel;
    //Configurar el StatusPanel con los stats actuales
    this.statusPanel.SetStats(this.idName, this.GetCurrentStats());
    return this;

    }

}
