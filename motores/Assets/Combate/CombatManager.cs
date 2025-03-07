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
    private Fighter[] playerTeam;
    private Fighter[] enemyTeam;

    private Fighter[] fighters;
    private int fighterIndex;

    private bool isCombatActive;

    private CombatStatus combatStatus;
    public EnemiesPanel enemiesPanel;
    public PlayerSkillPanel skillPanel;
    public StatusPanel statusPanel;
    private Skill currentFighterSkill;

    private List<Fighter> returnBuffer;

    private GameManager gameManagerInstance;

    public List<PlayerFighter> playerFighters = new List<PlayerFighter>();
    public List<EnemyFighter> enemyFighters = new List<EnemyFighter>();
    [SerializeField]
    private int countEnemyStart = 0;

    internal int opposingEnemyIndex = 999;


    void Start()
    {
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
        PlayerFighter playerFighter = playerCharacterInstance.GetComponent<PlayerFighter>().GetSkillPanel(skillPanel, statusPanel, enemiesPanel);

        PotionsInGame potionsScript = GameObject.FindObjectOfType<PotionsInGame>();
        if (potionsScript != null)
        {
            potionsScript.SetPlayerFighter(playerFighter);
        }



        this.fighters = GameObject.FindObjectsOfType<Fighter>();
        playerFighter.skillPanel = this.skillPanel;
        this.SortFightersBySpeed();
        this.MakeTeams();

        LogPanel.Write("Battle initiated.");

        this.combatStatus = CombatStatus.NEXT_TURN;

        this.fighterIndex = -1;
        this.isCombatActive = true;
        gameManagerInstance = GameObject.FindObjectOfType<GameManager>();
        this.returnBuffer = new List<Fighter>();

        StartCoroutine(this.CombatLoop());
    }

    private void SortFightersBySpeed()
    {
        bool sorted = false;
        while (!sorted)
        {
            sorted = true;

            for (int i = 0; i < this.fighters.Length - 1; i++)
            {
                Fighter a = this.fighters[i];
                Fighter b = this.fighters[i + 1];

                float aSpeed = a.GetCurrentStats().speed;
                float bSpeed = b.GetCurrentStats().speed;

                if (bSpeed > aSpeed)
                {
                    this.fighters[i] = b;
                    this.fighters[i + 1] = a;

                    sorted = false;
                }
            }
        }
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
                    bool arePlayersAlive = false;
                    foreach (var figther in this.playerTeam)
                    {
                        arePlayersAlive |= figther.isAlive;
                    }

                    // if (this.playerTeam[0].isAlive OR this.playerTeam[1].isAlive)

                    bool areEnemiesAlive = false;
                    foreach (var figther in this.enemyTeam)
                    {
                        areEnemiesAlive |= figther.isAlive;
                    }

                    bool victory = areEnemiesAlive == false;
                    bool defeat = arePlayersAlive == false;

                    GameManager.Instance.sumarcoinst(30);
                    if (GameManager.Instance.level < 1)
                    {
                        yield return new WaitForSeconds(0.5f);
                        GameManager.Instance.level++;
                    }

                    if (victory)
                    {
                        LogPanel.Write("Victoria!");
                        this.isCombatActive = false;
                        PlayerData.Get().SaveGame();
                        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                        SceneManager.LoadScene(currentSceneIndex + 1);
                    }

                    if (defeat)
                    {
                        LogPanel.Write("Derrota!");
                        this.isCombatActive = false;
                        SceneManager.LoadSceneAsync(5);
                    }

                    if (this.isCombatActive)
                    {
                        this.combatStatus = CombatStatus.NEXT_TURN;
                    }

                    yield return null;
                    break;
                case CombatStatus.NEXT_TURN:
                    yield return new WaitForSeconds(0.5f);

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
                            yield return new WaitForSeconds(0.5f);
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
