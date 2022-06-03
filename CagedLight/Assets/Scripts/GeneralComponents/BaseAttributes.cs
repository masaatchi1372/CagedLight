using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttributes
{
    public float health = 100;
    public float speed = 3f;
    public float attackCritChance;
    public float magicCritChance;
    public CharacterStrengthAttribute strengthAttr;
}
