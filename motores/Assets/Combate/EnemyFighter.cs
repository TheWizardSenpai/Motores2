using UnityEngine;
using System.Collections;

public class EnemyFighter : Fighter
{
    public int EnemyIndex;
    public EnemyDataBase EnemyDateBase;
    void Awake()
    {
        var data = EnemyDateBase.EnemyDB[EnemyIndex];
        //_IAEnemySimple = gameObject.GetComponent<IAEnemySimple>();
        //

        if (data.level != 0)
            this.stats = new Stats(data.level, data.maxHealth, data.attack, data.deffense, data.spirit, data.speed);
        else
            this.stats = new Stats(20, 50, 40, 30, 60, 10);
    }

    public override void InitTurn()
    {
        StartCoroutine(this.IA());

    }

    IEnumerator IA()
    {
        yield return new WaitForSeconds(1f);

        Skill skill = this.skills[Random.Range(0, this.skills.Length)];
        skill.SetEmitter(this);

        if (skill.needsManualTargeting)
        {
            Fighter[] targets = this.GetSkillTargets(skill);

            Fighter target = targets[Random.Range(0, targets.Length)];

            skill.AddReceiver(target);
            Invoke(nameof(ResetAnimation), 0.5f);
        }
        else
        {
            this.AutoConfigureSkillTargeting(skill);
        }

        this.combatManager.OnFighterSkill(skill);
    }

    private void ResetAnimation()
    {
        animator.Play("IDLE"); // Cambia la animaci?n a Idle
    }
}

