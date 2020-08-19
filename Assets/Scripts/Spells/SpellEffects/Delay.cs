using UnityEngine;
using System.Collections;

public class Delay : SpellEffect {
   public float delay = 1f;

   public override void Cast() {
      StartCoroutine(WaitThenCast(delay));
   }

   private IEnumerator WaitThenCast(float delay) {
      yield return new WaitForSeconds(delay);
      base.Cast();
   }
}