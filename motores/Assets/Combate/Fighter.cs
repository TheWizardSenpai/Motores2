using UnityEngine;
using System.Collections.Generic;

public abstract class Fighter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // Asignar el SpriteRenderer en el Inspector
    public string idName;
    public StatusPanel statusPanel;

    public CombatManager combatManager;
    public List<StatusMod> statusMods;
    public Stats stats;
    public Animator animator;
    [SerializeField]
    public Transform DamagePivot;

    protected Skill[] skills;

    public bool isAlive
    {
        get => this.stats.health > 0;
    }

    protected virtual void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        this.statusPanel.SetStats(this.idName, this.stats);
        this.skills = this.GetComponentsInChildren<Skill>();
        this.statusMods = new List<StatusMod>();
    }

    // Aquí es donde agregas el método para asignar el sprite desde los objetos desactivados
    public void SetCharacterSpriteFromDatabase(Character character)
    {
        // Buscar el prefab desde la base de datos de personajes
        GameObject spriteObject = Instantiate(character.characterPrefab);  // Instanciamos el prefab

        if (spriteObject != null)
        {
            // Activamos el objeto con el sprite
            spriteObject.SetActive(true);

            // Obtener el SpriteRenderer del objeto
            SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                // Asignamos el sprite al SpriteRenderer del personaje
                this.spriteRenderer.sprite = spriteRenderer.sprite;
            }
            else
            {
                Debug.LogError("No se encontró el SpriteRenderer para el personaje: " + character.characterName);
            }
        }
        else
        {
            Debug.LogError("No se encontró el GameObject para el personaje: " + character.characterName);
        }
    }

    protected void Die()
    {
        this.statusPanel.gameObject.SetActive(false);
        this.gameObject.SetActive(false);

        if (this.isAlive == false)
        {
            animator.Play("Muelte");
            Invoke("Die", 2f);
        }
    }

    public void ModifyHealth(float amount)
    {
        this.stats.health = Mathf.Clamp(this.stats.health + amount, 0f, this.stats.maxHealth);
        this.stats.health = Mathf.Round(this.stats.health);
        this.statusPanel.SetHealth(this.stats.health, this.stats.maxHealth);

        if (this.isAlive == false)
        {
            Invoke("Die", 2f);
        }
    }

    public Stats GetCurrentStats()
    {
        Stats modedStats = this.stats;

        foreach (var mod in this.statusMods)
        {
            modedStats = mod.Apply(modedStats);
        }

        return modedStats;
    }

    public void SetCharacterSprite(Sprite characterSprite)
    {
        Debug.Log("SetCharacterSprite ejecutado con sprite: " + characterSprite.name);

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = characterSprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer no asignado en " + this.name);
        }
    }
    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public abstract void InitTurn();
}
