using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory 
{
    private List<Potion> potions = new List<Potion>();

    private const int maxPotions = 9;

    public bool addPotions (Potion potion)
    {
        if(potions.Count < maxPotions)
        {
            potions.Add(potion);
            return true;
        }
        return false;
    }

    public void UsePotion(Potion potion)
    {
        potions.Remove(potion);
    }
}
