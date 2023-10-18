using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
        
    [SerializeField] private int coins;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void sumarcoinst(int coin)
    {
        coins += coin;
    }

}

