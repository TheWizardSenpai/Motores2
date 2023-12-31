using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float stamina;
    public float currency;
    public int level;
    public string nextStamina;
    public string lastStamina;
    public bool viewTutorial;

    public bool isFirstTime = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }  

    public void sumarcoinst(int coin)
    {
        currency += coin;
    } 
}

