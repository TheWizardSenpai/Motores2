using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PotionsInGame : MonoBehaviour
{
    private readonly Inventory inventory = new Inventory();

    public GameObject[] potionPrefabs;
    public Transform potionsParent;
    public PlayerFighter playerFighter;

    private void Start()
    {
        inventory.LoadInventory();
        InstantiatePotions();
    }

    private void InstantiatePotions()
    {
        foreach (Potion potion in inventory.GetPotions())
        {
            if (potion.Quantity > 0)
            {
                int prefabIndex = (int)potion.potionType;
                GameObject potionPrefab = potionPrefabs[prefabIndex];
                GameObject potionInstance = Instantiate(potionPrefab, potionsParent);
                TextMeshProUGUI quantityText = potionInstance.GetComponentInChildren<TextMeshProUGUI>();
                quantityText.text = potion.Quantity.ToString();

                Button potionButton = potionInstance.GetComponentInChildren<Button>();
                potionButton.onClick.AddListener(delegate { UsePotion(potion, quantityText, potionButton); });
            }
        }
    }


    private void UsePotion(Potion potion, TextMeshProUGUI quantityText, Button potionButton)
    {
        Debug.Log("Using potion");
        inventory.UsePotion(potion.potionType);

        int newQuantity = potion.Quantity;
        quantityText.text = newQuantity.ToString();

        playerFighter.ExecuteSkill(2);

        if (newQuantity <= 0)
        {
            potionButton.interactable = false;
        }

        inventory.SaveInventory();
    }
}
