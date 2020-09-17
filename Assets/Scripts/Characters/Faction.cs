using System.Collections.Generic;
using UnityEngine;

public class Faction : MonoBehaviour {
    public List<Faction> enemies = new List<Faction>();

    public List<Health> members = new List<Health>();

    public Health GetMember(Vector3 position, float range) {
        Health closest = null;
        float closestDist = float.MaxValue;
        for (int i = 0; i < members.Count; i++) {
            float dist = (position - members[i].transform.position).sqrMagnitude;
            if (dist < range * range && dist < closestDist) {
                closest = members[i];
                closestDist = dist;
            }
        }

        return closest;
    }

    public Health GetEnemyMember(Vector3 position, float range) {
        Health closest = null;
        float closestDist = float.MaxValue;
        for (int i = 0; i < enemies.Count; i++) {
            Health closestInFaction = enemies[i].GetMember(position, range);
            if (closestInFaction == null) 
                continue;

            float dist = (position - closestInFaction.transform.position).sqrMagnitude;
            if (dist < closestDist) {
                closest = closestInFaction;
                closestDist = dist;
            }
        }

        return closest;
    }
}