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
    private GameManager gameManagerInstance;

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
            Debug.LogError("No se encontró el objeto GameManager en la escena.");
            return;
        }

        int selectedCharacterIndex = PlayerPrefs.GetInt("selectedOption", 0);
        CharacterDatabase characterDB = Resources.Load<CharacterDatabase>("CharacterDatabase");

        if (characterDB == null)
        {
            Debug.LogError("No se encontró CharacterDatabase en Resources.");
            return;
        }
        
        GameObject personajePosition = GameObject.Find("Personaje");
        Character selectedCharacter = characterDB.GetCharacter(selectedCharacterIndex);

        if (selectedCharacter.characterPrefab == null)
        {
            Debug.LogError("El prefab del personaje no está asignado en el CharacterDatabase.");
            return;
        }

        GameObject playerCharacterInstance = Instantiate(selectedCharacter.characterPrefab, personajePosition.transform.position, personajePosition.transform.rotation);

        // ?? Instanciar el prefab del personaje en combate
        //GameObject playerCharacterInstance = Instantiate(selectedCharacter.characterPrefab, transform.position, Quaternion.identity);
        playerCharacterInstance.SetActive(true); // ? Activar el prefab después de instanciarlo
        Animator characterAnimator = playerCharacterInstance.GetComponent<Animator>();
        // ?? Asignar la referencia del prefab instanciado a playerTear

        if (playerTeam == null)
        {
            Debug.LogError("El prefab instanciado no tiene un componente PlayerFighter.");
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

                    currentFighterSkill.Run();

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
                            if (GameManager.Instance.level < 1)
                            {
                                yield return new WaitForSeconds(2f);
                                GameManager.Instance.level++;
                            }
                            PlayerData.Get().SaveGame();
                            LogPanel.Write("Victory!");
                            this.isCombatActive = false;

                            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

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
        this.combatStatus = CombatStatus.SKIP_TURN;
    }
}
