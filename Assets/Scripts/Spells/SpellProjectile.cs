using UnityEngine;
using Mirror;

public class SpellProjectile : NetworkBehaviour {
   public new Rigidbody rigidbody;
   public SpellEffect onHitEffect;

   public bool dieOnHit;

   [ServerCallback]
   private void OnCollisionEnter(Collision other) {
      var h = other.gameObject.GetComponent<Health>();
      if (h) {
         onHitEffect.HitHealth(h);
      }
      onHitEffect.HitAny(other.gameObject);
      if (dieOnHit)
         ObjectPool.singleton.UnSpawnObject(this.gameObject);
   }
}