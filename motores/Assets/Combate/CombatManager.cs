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
        gameManagerInstance = GameObject.FindObjectOfType<GameManager>();

        if (gameManagerInstance == null)
        {
            Debug.LogError("No se encontr� el objeto GameManager en la escena.");
            return;
        }

        // Obtener el �ndice del personaje seleccionado guardado en PlayerPrefs
        int selectedCharacterIndex = PlayerPrefs.GetInt("selectedOption", 0);

        // Cargar el CharacterDatabase (aseg�rate de tener el asset en Resources)
        CharacterDatabase characterDB = Resources.Load<CharacterDatabase>("CharacterDatabase");

        if (characterDB == null)
        {
            Debug.LogError("No se encontr� CharacterDatabase en Resources.");
            return;
        }

        // Obtener el Character seleccionado
        Character selectedCharacter = characterDB.GetCharacter(selectedCharacterIndex);

        // Asignar el sprite y nombre al jugador (playerTeam)
        if (playerTeam is PlayerFighter playerFighter)
        {
            playerFighter.SetCharacterSpriteFromDatabase(selectedCharacter);
        }
        else
        {
            Debug.LogError("playerTeam no es una instancia de PlayerFighter.");
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
                    this.combatStatus = CombatStatus.CHECK_ACTION_MESSAGES;
                    break;
                case CombatStatus.CHECK_ACTION_MESSAGES:
                    string nextMessage = this.currentFighterSkill.GetNextMessage();
                    if (nextMessage != null)
                    {
                        LogPanel.Write(nextMessage);
                        yield return new WaitForSeconds(0.5f);
                    }
                    else
                    {
                        this.currentFighterSkill = null;
                        this.combatStatus = CombatStatus.CHECK_FOR_VICTORY;
                        yield return null;
                    }

                    break;

                case CombatStatus.CHECK_FOR_VICTORY:

                    foreach (var fgtr in this.fighters)
                    {
                        if (fgtr.isAlive == false)
                        {
                            GameManager.Instance.sumarcoinst(30);
                            if (GameManager.Instance.level < 1) // para que no vaya al level 3 ni 4 etc
                            {
                                yield return new WaitForSeconds(2f);
                                GameManager.Instance.level++;
                            }
                            PlayerData.Get().SaveGame();
                            LogPanel.Write("Victory!");
                            this.isCombatActive = false;
                            //SceneManager.LoadScene(2);

                            // Obtener el �ndice de la escena actual
                            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

                            // Cargar la siguiente escena en orden
                            SceneManager.LoadScene(currentSceneIndex + 1);


                        }

                        if (playerTeam.isAlive == false)
                        {
                            SceneManager.LoadScene(4);
                        }

                        else
                        {
                            this.combatStatus = CombatStatus.NEXT_TURN;
                        }
                    }
                    yield return null;
                    break;
                case CombatStatus.NEXT_TURN:
                    yield return new WaitForSeconds(0.5f);

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