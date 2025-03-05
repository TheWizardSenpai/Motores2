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
    CHECK_FIGHTER_STATUS_CONDITION
}

public class CombatManager : MonoBehaviour
{
    private GameManager gameManagerInstance;
    private Fighter[] playerTeam;
    private Fighter[] enemyTeam;
    public Fighter[] fighters;
    private int fighterIndex;
    private bool isCombatActive;
    public GameObject statusPanelPrefab;

    private CombatStatus combatStatus;

    private Skill currentFighterSkill;
    private List<Fighter> returnBuffer;

    public int FighterIndex { get => fighterIndex; }

    public List<PlayerFighter> playerFighters = new List<PlayerFighter>();
    public List<EnemyFighter> enemyFighters = new List<EnemyFighter>();
    [SerializeField]
    private int countEnemyStart = 0;

    internal int opposingEnemyIndex = 999;


    void Start()
    {
        foreach (var fighter in fighters)
        {
            fighter.statusPanel = Instantiate(statusPanelPrefab, fighter.transform).GetComponent<StatusPanel>();
        }
        gameManagerInstance = GameObject.FindObjectOfType<GameManager>();
        this.returnBuffer = new List<Fighter>();
        if (gameManagerInstance == null)
        {
            Debug.LogError("No se encontr? el objeto GameManager en la escena.");
            return;
        }
        int selectedCharacterIndex = PlayerPrefs.GetInt("selectedOption", 0);
        CharacterDatabase characterDB = Resources.Load<CharacterDatabase>("CharacterDatabase");


        if (characterDB == null)
        {
            Debug.LogError("No se encontr? CharacterDatabase en Resources.");
            return;
        }

        GameObject personajePosition = GameObject.Find("Personaje");
        Character selectedCharacter = characterDB.GetCharacter(selectedCharacterIndex);
        if (selectedCharacter.characterPrefab == null)
        {
            Debug.LogError("El prefab del personaje no est? asignado en el CharacterDatabase.");
            return;
        }

        GameObject playerCharacterInstance = Instantiate(selectedCharacter.characterPrefab, personajePosition.transform.position, personajePosition.transform.rotation);
        playerCharacterInstance.SetActive(true); // ? Activar el prefab despu?s de instanciarlo
        Animator characterAnimator = playerCharacterInstance.GetComponent<Animator>();

        this.fighters = GameObject.FindObjectsOfType<Fighter>();
        this.MakeTeams();

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
        countEnemyStart = enemyFighters.Count;
        this.fighterIndex = -1;
        this.isCombatActive = true;
        StartCoroutine(this.CombatLoop());
    }

    private void MakeTeams()
    {
        List<Fighter> playersBuffer = new List<Fighter>();
        List<Fighter> enemiesBuffer = new List<Fighter>();
        foreach (var fgtr in this.fighters)
        {
            if (fgtr.team == Team.PLAYERS)
            {
                playersBuffer.Add(fgtr);
            }
            else if (fgtr.team == Team.ENEMIES)
            {
                enemiesBuffer.Add(fgtr);
            }
            fgtr.combatManager = this;
        }
        this.playerTeam = playersBuffer.ToArray();
        this.enemyTeam = enemiesBuffer.ToArray();
    }
    internal void removeFighter(PlayerFighter playerFighter)
    {
        throw new System.NotImplementedException();
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
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        this.currentFighterSkill = null;
                        this.combatStatus = CombatStatus.CHECK_FOR_VICTORY;
                        yield return null;
                    }
                    break;
                case CombatStatus.CHECK_FOR_VICTORY:
                    var countEnemyDown = 0;
                    GameManager.Instance.sumarcoinst(30);
                    if (GameManager.Instance.level < 1)
                    {
                        yield return new WaitForSeconds(2f);
                        GameManager.Instance.level++;
                    }
                    foreach (var fgtr in this.enemyFighters)
                    {
                        if (fgtr.isAlive == false)
                        {
                            countEnemyDown += 1;
                        }
                    }
                    if (countEnemyDown == countEnemyStart)
                    {
                        this.isCombatActive = false;

                        LogPanel.Write("Victory!");
                        PlayerData.Get().SaveGame();
                        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

                        SceneManager.LoadScene(currentSceneIndex + 1);

                    }
                    else
                    {
                        this.combatStatus = CombatStatus.NEXT_TURN;
                    }
                    yield return null;
                    break;
                case CombatStatus.NEXT_TURN:
                    yield return new WaitForSeconds(1f);
                    Fighter current = null;

                    do
                    {
                        this.fighterIndex = (this.fighterIndex + 1) % this.fighters.Length;

                        current = this.fighters[this.fighterIndex];
                    } while (current.isAlive == false);

                    this.combatStatus = CombatStatus.CHECK_FIGHTER_STATUS_CONDITION;

                    break;

                case CombatStatus.CHECK_FIGHTER_STATUS_CONDITION:

                    var currentFighter = this.fighters[this.fighterIndex];

                    var statusCondition = currentFighter.GetCurrentStatusCondition();

                    if (statusCondition != null)
                    {
                        statusCondition.Apply();
                        while (true)
                        {
                            string nextSCMessage = statusCondition.GetNextMessage();
                            if (nextSCMessage == null)
                            {
                                break;
                            }
                            LogPanel.Write(nextSCMessage);
                            yield return new WaitForSeconds(2f);
                        }
                        if (statusCondition.BlocksTurn())
                        {
                            this.combatStatus = CombatStatus.CHECK_FOR_VICTORY;
                            break;
                        }
                    }
                    LogPanel.Write($"{currentFighter.idName} has the turn.");
                    currentFighter.InitTurn();

                    this.combatStatus = CombatStatus.WAITING_FOR_FIGHTER;
                    break;

            }
        }
    }

    public Fighter GetOpposingCharacter()
    {
        foreach (var playerFighter in this.playerFighters)
        {
            {
                if (playerFighter.GetCurrentStats().health > 0)
                {
                    return playerFighter;
                }
            }
        }
        return playerFighters[0];
    }
    public Fighter[] FilterJustAlive(Fighter[] team)
    {
        this.returnBuffer.Clear();

        foreach (var fgtr in team)
        {
            if (fgtr.isAlive)
            {
                this.returnBuffer.Add(fgtr);
            }
        }

        return this.returnBuffer.ToArray();
    }

    public Fighter[] GetOpposingTeam()
    {
        Fighter currentFighter = this.fighters[this.fighterIndex];

        Fighter[] team = null;
        if (currentFighter.team == Team.PLAYERS)
        {
            team = this.enemyTeam;
        }
        else if (currentFighter.team == Team.ENEMIES)
        {
            team = this.playerTeam;
        }

        return this.FilterJustAlive(team);
    }


    public Fighter[] GetAllyTeam()
    {
        Fighter currentFighter = this.fighters[this.fighterIndex];

        Fighter[] team = null;
        if (currentFighter.team == Team.PLAYERS)
        {
            team = this.playerTeam;
        }
        else
        {
            team = this.enemyTeam;
        }

        return this.FilterJustAlive(team);
    }

    public void OnFighterSkill(Skill skill)
    {
        this.currentFighterSkill = skill;
        this.combatStatus = CombatStatus.FIGHTER_ACTION;
    }
}