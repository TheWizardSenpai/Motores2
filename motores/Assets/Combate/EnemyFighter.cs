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
            this.stats = new Stats(data.level, data.maxHealth, data.attack, data.deffense, data.spirit);
        else
            this.stats = new Stats(20, 50, 40, 30, 60);
    }

    public override void InitTurn()
    {
        if (stats.health > 0)
            StartCoroutine(this.IA());
        else
            StartCoroutine(this.Death());

    }

    IEnumerator IA()
    {
        yield return new WaitForSeconds(1f);
        animator.Play("Attack");
        Skill skill = this.skills[Random.Range(0, this.skills.Length)];
        yield return new WaitForSeconds(0.5f);
        animator.Play("IDLE");
;
        skill.SetEmitterAndReceiver(
            this, this.combatManager.GetOpposingCharacter());

        this.combatManager.OnFighterSkill(skill);
    }

    IEnumerator Death()
    {
        animator.Play("Muelte");
        yield return new WaitForSeconds(1f);



        this.combatManager.OnFighterDead();

    }
}