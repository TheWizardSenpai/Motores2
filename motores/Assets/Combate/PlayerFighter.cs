using UnityEngine;

public class PlayerFighter : Fighter
{
    [Header("UI")]
    public PlayerSkillPanel skillPanel;

    void Awake()
    {
        this.stats = new Stats(21, 60, 50, 45, 20);
    }

    public override void InitTurn()
    {
        this.skillPanel.Show();

        for (int i = 0; i < this.skills.Length; i++)
        {
            this.skillPanel.ConfigureButtons(i, this.skills[i].skillName);
        }
    }

    public void ExecuteSkill(int index)
    {
        this.skillPanel.Hide();
        animator.Play("Attack");
        Skill skill = this.skills[index];

        // EXECUTE
        skill.SetEmitterAndReceiver(this, this.combatManager.GetOpposingCharacter());
        this.combatManager.OnFighterSkill(skill);

        Invoke(nameof(ResetAnimation), 0.5f);
    }

    private void ResetAnimation()
    {
        animator.Play("IDLE");
    }
}
