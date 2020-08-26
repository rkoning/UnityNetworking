using UnityEngine;

public class AimAtLookPoint : SpellEffect {
   public Transform overrideTransform;
   public override void Cast() {
      if (overrideTransform)
         overrideTransform.rotation = Quaternion.LookRotation(spell.owner.lookPoint - overrideTransform.position);
      else 
         transform.rotation = Quaternion.LookRotation(spell.owner.lookPoint - transform.position);
   }
}