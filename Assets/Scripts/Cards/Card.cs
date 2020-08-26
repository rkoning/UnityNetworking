using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "ScriptableObjects/Card", order = 0)]
public class Card : ScriptableObject
{
    public int id;
    public GameObject spellPrefab;
    public Sprite sprite;
    public string description;
    public float manaCost;
    public Lore lore;

    public bool consume;
    public bool collectable = true;
}

