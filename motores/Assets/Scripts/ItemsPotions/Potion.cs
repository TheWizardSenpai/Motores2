using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion
{
    public enum PotionType { Type1, Type2, Type3, Type4 }
    public PotionType potionType;
    public int Quantity { get; private set; }

    public Potion(PotionType type, int quantity = 1)
    {
        potionType = type;
        Quantity = quantity;
    }

    public void AddQuantity(int amount)
    {
        Quantity += amount;
    }

    public void UsePotion()
    {
        if (Quantity > 0)
        {
            Quantity--;
        }
    }
}

