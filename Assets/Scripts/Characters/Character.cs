using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "ScriptableObjects/Character", order = 1)]
public class Character : ScriptableObject
{
    public GameObject avatarPrefab;
}
