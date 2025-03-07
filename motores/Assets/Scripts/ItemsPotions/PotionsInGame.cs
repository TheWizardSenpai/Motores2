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

        // Usamos la poci�n en el inventario
        inventory.UsePotion(potion.potionType);

        // Obtener la salud m�xima y la cantidad de curaci�n basada en el tipo de poci�n
        Stats playerStats = playerFighter.GetStats();
        float healingAmount = 0f;

        // Definir la cantidad de curaci�n seg�n el tipo de poci�n
        switch (potion.potionType)
        {
            case Potion.PotionType.Type1:
                healingAmount = playerStats.maxHealth * 0.2f; // 20% de la salud m�xima
                break;
            case Potion.PotionType.Type2:
                healingAmount = playerStats.maxHealth * 0.5f; // 50% de la salud m�xima
                break;
            case Potion.PotionType.Type3:
                healingAmount = 100f; // Una cantidad fija, por ejemplo 100 de vida
                break;
            case Potion.PotionType.Type4:
                healingAmount = 200f; // Otra cantidad fija para otro tipo de poci�n
                break;
        }

        // Curar al jugador, asegur�ndose de no superar la salud m�xima
        playerFighter.ModifyHealth(healingAmount);

        // Actualizar la cantidad de la poci�n mostrada en el UI
        int newQuantity = potion.Quantity;
        quantityText.text = newQuantity.ToString();

        // Si ya no hay m�s pociones, desactivamos el bot�n
        if (newQuantity <= 0)
        {
            potionButton.interactable = false;
        }

        // Guardar el inventario actualizado
        inventory.SaveInventory();

        // Opcional: Agregar efectos visuales o de sonido al usar la poci�n
    }
 }


