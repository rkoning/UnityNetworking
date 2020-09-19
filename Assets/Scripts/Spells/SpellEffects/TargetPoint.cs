using System.Net.NetworkInformation;
using UnityEngine;

public class TargetPoint : SpellEffect {
   public float maxRange = 100f;
   public LayerMask mask;

   public override void Cast() {
      var head = spell.owner.aimTransform;
      RaycastHit hit;
      if (Physics.Raycast(head.position, head.forward, out hit, maxRange, mask)) {
         transform.position = hit.point;
         transform.rotation = Quaternion.identity;
         base.Cast();
      }
   }
}