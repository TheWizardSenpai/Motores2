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
        // Asegurarse de que el spriteRenderer esté asignado
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer no asignado en " + this.name);
            }
        }

        this.statusPanel.SetStats(this.idName, this.stats);
        this.skills = this.GetComponentsInChildren<Skill>();
        this.statusMods = new List<StatusMod>();
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
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = characterSprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer no asignado en " + this.name);
        }
    }

    public abstract void InitTurn();
}