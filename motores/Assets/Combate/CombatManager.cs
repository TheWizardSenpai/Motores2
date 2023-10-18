using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum CombatStatus
{
    WAITING_FOR_FIGHTER,
    FIGHTER_ACTION,
    CHECK_ACTION_MESSAGES,
    CHECK_FOR_VICTORY,
    NEXT_TURN,
    SKIP_TURN
}

public class CombatManager : MonoBehaviour
{
    private GameManager gameManagerInstance; // Variable para almacenar la instancia del GameManager


    public Fighter[] fighters;
    [SerializeField]
    private int fighterIndex;

    public Fighter playerTeam;
    public Fighter enemyTeam;
    public int FighterIndex { get => fighterIndex; }

    private bool isCombatActive;

    private CombatStatus combatStatus;

    private Skill currentFighterSkill;


    void Start()
    {
        gameManagerInstance = GameObject.FindObjectOfType<GameManager>(); // Encuentra la instancia existente del GameManager en la escena

        if (gameManagerInstance == null)
        {
            Debug.LogError("No se encontró el objeto GameManager en la escena.");
            return;
        }
        LogPanel.Write("Battle initiated.");

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
                case CombatStatus.CHECK_ACTION_MESSAGES:
                    string nextMessage = this.currentFighterSkill.GetNextMessage();
                    if (nextMessage != null)
                    {
                        LogPanel.Write(nextMessage);
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        this.currentFighterSkill = null;
                        this.combatStatus = CombatStatus.CHECK_FOR_VICTORY;
                        yield return new WaitForSeconds(2f);
                    }

                    break;

                case CombatStatus.CHECK_FOR_VICTORY:
                    
                    foreach (var fgtr in this.fighters)
                    {
                        if (fgtr.isAlive == false)
                        {
                            GameManager.Instance.sumarcoinst(5);
                            LogPanel.Write("Victory!");
                            this.isCombatActive = false;
                            SceneManager.LoadScene(2);
                        }

                        if (playerTeam.isAlive == false)
                        {
                            SceneManager.LoadScene(3);
                        }
                    
                    else
                    {
                        this.combatStatus = CombatStatus.NEXT_TURN;
                    }
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
                    
            }
        }
    }

    public Fighter GetOpposingCharacter()
    {
        if (this.fighterIndex == 0)
        {
            return this.fighters[1];
        }
        else
        {
            return this.fighters[0];
        }
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