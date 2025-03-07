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
        playerFighter = FindObjectOfType<PlayerFighter>();
        playerFighter = GameObject.Find("PlayerFighter(Clone)").GetComponent<PlayerFighter>();
    }

    private void InstantiatePotions()
    {
        foreach (Potion potion in inventory.GetPotions())
        {
            if (potion != null && potion.Quantity > 0)
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
    public void SetPlayerFighter(PlayerFighter fighter)
    {
        playerFighter = fighter;
    }



    private void UsePotion(Potion potion, TextMeshProUGUI quantityText, Button potionButton)
    {
        Debug.Log("Using potion");

        // Usamos la poción en el inventario
        inventory.UsePotion(potion.potionType);

        // Obtener la salud máxima y la cantidad de curación basada en el tipo de poción
        Stats playerStats = playerFighter.GetStats();
        float healingAmount = 0f;

        // Definir la cantidad de curación según el tipo de poción
        switch (potion.potionType)
        {
            case Potion.PotionType.Type1:
                healingAmount = playerStats.maxHealth * 0.2f; // 20% de la salud máxima
                break;
            case Potion.PotionType.Type2:
                healingAmount = playerStats.maxHealth * 0.5f; // 50% de la salud máxima
                break;
            case Potion.PotionType.Type3:
                healingAmount = 100f; // Una cantidad fija, por ejemplo 100 de vida
                break;
            case Potion.PotionType.Type4:
                healingAmount = 200f; // Otra cantidad fija para otro tipo de poción
                break;
        }

        // Curar al jugador, asegurándose de no superar la salud máxima
        playerFighter.ModifyHealth(healingAmount);

        // Actualizar la cantidad de la poción mostrada en el UI
        int newQuantity = potion.Quantity;
        quantityText.text = newQuantity.ToString();

        // Si ya no hay más pociones, desactivamos el botón
        if (newQuantity <= 0)
        {
            potionButton.interactable = false;
        }

        // Guardar el inventario actualizado
        inventory.SaveInventory();

        // Opcional: Agregar efectos visuales o de sonido al usar la poción
    }
 }


