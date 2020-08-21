using UnityEngine;

public class ExcludeSelf : SpellEffect {
   public override void HitAny(GameObject other) {
      if (other.gameObject != spell.owner.gameObject) {
         base.HitAny(other);
      }
   }

   public override void HitHealth(Health target) {
      if (target.gameObject != spell.owner.gameObject) {
         base.HitHealth(target);
      }
   }
}