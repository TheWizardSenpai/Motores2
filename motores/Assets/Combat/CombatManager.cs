using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CombatStatus
{
    WAITING_FOR_FIGHTER,
    FIGHTER_ACTION,
    CHECK_FOR_VICTORY,
    NEXT_TURN,
    SKIP_TURN
}

public class CombatManager : MonoBehaviour
{
    public Fighter[] fighters;
    [SerializeField]
    private int fighterIndex;

    public int FighterIndex { get => fighterIndex; }

    private bool isCombatActive;

    private CombatStatus combatStatus;

    private Skill currentFighterSkill;

    public List<PlayerFighter> playerFighters = new List<PlayerFighter>();
    public List<EnemyFighter> enemyFighters = new List<EnemyFighter>();

    void Start()
    {
        LogPanel.Write("Battle initiated.");

        foreach (var fgtr in this.fighters)
        {
            fgtr.combatManager = this;

            if (fgtr.GetType() == typeof(PlayerFighter))
                playerFighters.Add(fgtr.GetComponent<PlayerFighter>());

            if (fgtr.GetType() == typeof(EnemyFighter))
                enemyFighters.Add(fgtr.GetComponent<EnemyFighter>());
        }

        this.combatStatus = CombatStatus.NEXT_TURN;

        this.fighterIndex = -1;
        this.isCombatActive = true;
        StartCoroutine(this.CombatLoop());
    }

    IEnumerator CombatLoop()
    {
        while (this.isCombatActive)
        {
            switch (this.combatStatus)
            {
                case CombatStatus.WAITING_FOR_FIGHTER:
                    yield return null;
                    break;

                case CombatStatus.FIGHTER_ACTION:
                    LogPanel.Write($"{this.fighters[this.fighterIndex].idName} uses {currentFighterSkill.skillName}.");

                    yield return null;

                    // Executing fighter skill
                    currentFighterSkill.Run();

                    // Wait for fighter skill animation
                    yield return new WaitForSeconds(currentFighterSkill.animationDuration);
                    this.combatStatus = CombatStatus.CHECK_FOR_VICTORY;

                    currentFighterSkill = null;
                    break;

                case CombatStatus.CHECK_FOR_VICTORY:
                    var countEnemyDown = 0;
                    foreach (var fgtr in this.enemyFighters)
                    {
                        if (fgtr.isAlive == false)
                        {
                            countEnemyDown += 1;
                        }
                    }
                    if (countEnemyDown == enemyFighters.Count)
                    {
                        this.isCombatActive = false;

                        LogPanel.Write("Victory!");
                    }
                    else
                    {
                        this.combatStatus = CombatStatus.NEXT_TURN;
                    }
                    yield return null;
                    break;
                case CombatStatus.NEXT_TURN:
                    yield return new WaitForSeconds(1f);
                    this.fighterIndex = (this.fighterIndex + 1) % this.fighters.Length;

                    var currentTurn = this.fighters[this.fighterIndex];

                    LogPanel.Write($"{currentTurn.idName} has the turn.");
                    currentTurn.InitTurn();

                    this.combatStatus = CombatStatus.WAITING_FOR_FIGHTER;

                    break;
                case CombatStatus.SKIP_TURN:
                    //yield return new WaitForSeconds(1f);
                    this.fighterIndex = (this.fighterIndex + 1) % this.fighters.Length;

                    currentTurn = this.fighters[this.fighterIndex];

                    //LogPanel.Write($"{currentTurn.idName} has the turn.");
                    currentTurn.InitTurn();

                    this.combatStatus = CombatStatus.WAITING_FOR_FIGHTER;

                    break;
            }
        }
    }

    public Fighter GetOpposingCharacter()
    {
        foreach (var playerFighter in this.playerFighters)
        {
            if (playerFighter.GetCurrentStats().health > 0)
            {
                return playerFighter;
            }
        }

        return playerFighters[0];
    }

    public Fighter GetOpposingEnemy()
    {
        foreach (var enemyFighter in this.enemyFighters)
        {
            if (enemyFighter.GetCurrentStats().health > 0)
            {
                return enemyFighter;
            }
        }

        return enemyFighters[0];
    }
    public void OnFighterSkill(Skill skill)
    {
        this.currentFighterSkill = skill;
        this.combatStatus = CombatStatus.FIGHTER_ACTION;
    }

    public void OnFighterDead()
    {
        //Debug.Log("Muerto");
        this.combatStatus = CombatStatus.SKIP_TURN;
    }
}