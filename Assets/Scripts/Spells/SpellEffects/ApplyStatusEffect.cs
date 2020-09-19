using UnityEngine;

public class ApplyStatusEffect : SpellEffect {
   public StatusFactory statusFactory;

   public override void HitHealth(Health other) {
      spell.owner.ApplyStatus(other, statusFactory);
   }
}