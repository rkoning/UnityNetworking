using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "ScriptableObjects/Character", order = 1)]
public class Character : ScriptableObject
{
    public int id;
    public string activeName;
    public string activeDescription;

    public string passiveName;
    public string passiveDescription;
    public Lore[] lores;

    public GameObject avatarPrefab;
}
