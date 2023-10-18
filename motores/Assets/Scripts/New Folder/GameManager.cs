using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float stamina;
    public float currency;
    public int level;

    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void sumarcoinst(int coin)
    {
        currency += coin;
    }

}

