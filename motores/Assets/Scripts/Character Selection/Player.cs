using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterDatabase characterDB;
    public SpriteRenderer artworkSprite;

    private int selectedOption = 0;
    void Start()
    {
        if (!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }

        UpdateCharacter(selectedOption);
    }
    private void UpdateCharacter(int selectedOption)
    {
        Character character = characterDB.GetCharacter(selectedOption);

        // Aquí accedes al prefab y obtienes el SpriteRenderer de ese prefab
        SpriteRenderer prefabSpriteRenderer = character.characterPrefab.GetComponent<SpriteRenderer>();

        if (prefabSpriteRenderer != null)
        {
            artworkSprite.sprite = prefabSpriteRenderer.sprite;
        }
        else
        {
            Debug.LogError("No se encontró el SpriteRenderer en el prefab del personaje: " + character.characterName);
        }


    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
    }

}
