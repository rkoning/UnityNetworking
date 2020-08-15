﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "ScriptableObjects/Card", order = 0)]
public class Card : ScriptableObject
{
    public GameObject spellPrefab;
    public string description;
    public int manaCost;
}