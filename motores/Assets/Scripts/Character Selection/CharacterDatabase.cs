using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Character Database", order = 1)]
public class CharacterDatabase : ScriptableObject
{
    public Character[] characters;  // Aquí van los prefabs

    public int CharacterCount
    {
        get
        {
            return characters.Length;
        }
    }

    public Character GetCharacter(int index)
    {
        return characters[index];
    }
}

