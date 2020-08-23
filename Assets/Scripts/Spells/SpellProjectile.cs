using UnityEngine;

public class SpellProjectile : MonoBehaviour, IPoolableObject {
   public new Rigidbody rigidbody;
   public SpellEffect onHitEffect;

   public bool dieOnHit;
   public void Init() {
      if (!rigidbody) {
         rigidbody = GetComponent<Rigidbody>();
      }
   }

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