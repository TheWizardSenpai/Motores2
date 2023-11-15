using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Potion> potions = new List<Potion>();
    private const int maxPotions = 9;

    public bool AddPotion(Potion potion)
    {
        foreach (var existingPotion in potions)
        {
            if (existingPotion.potionType == potion.potionType)
            {
                existingPotion.AddQuantity(potion.Quantity);
                return true;
            }
        }

        if (potions.Count < maxPotions)
        {
            potions.Add(potion);
            return true;
        }
        return false;
    }

    public void UsePotion(Potion.PotionType potionType)
    {
        foreach (var potion in potions)
        {
            if (potion.potionType == potionType)
            {
                potion.UsePotion();
                break;
            }
        }
    }

    public List<Potion> GetPotions()
    {
        return potions;
    }

    public void SaveInventory()
    {
        string inventoryData = "";
        foreach (Potion potion in potions)
        {
            inventoryData += potion.Quantity.ToString() + ",";
        }
        inventoryData = inventoryData.TrimEnd(',');

        PlayerPrefs.SetString("InventoryData", inventoryData);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        string savedData = PlayerPrefs.GetString("InventoryData", "");
        if (!string.IsNullOrEmpty(savedData))
        {
            string[] quantities = savedData.Split(',');
            for (int i = 0; i < quantities.Length; i++)
            {
                int quantity = int.Parse(quantities[i]);
                if (quantity > 0)
                {
                    potions.Add(new Potion((Potion.PotionType)i, quantity));
                }
            }
        }
    }

}

