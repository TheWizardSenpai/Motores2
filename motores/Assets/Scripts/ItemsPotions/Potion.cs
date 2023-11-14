using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion 
{
    public enum PotionType
    {
        Type1,
        Type2,
        Type3,
        Type4
    }

    public PotionType potionType;

    public Potion (PotionType type)
    {
        potionType = type;
    } 

}
