using System.Net.NetworkInformation;
using UnityEngine;

public class TargetPoint : SpellEffect {
   public float maxRange = 100f;
   public override void Cast() {
      if ((spell.owner.lookPoint - spell.owner.transform.position).sqrMagnitude > maxRange * maxRange)
         return;
      transform.position = spell.owner.lookPoint;
      transform.rotation = Quaternion.identity;
      base.Cast();
   }
}