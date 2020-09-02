using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class SpellProjectile : NetworkBehaviour, IPoolableObject {
   public new Rigidbody rigidbody;
   public SpellEffect onHitEffect;


   public bool dieOnHit;

   public void Init() { }

   public void RpcInit() {

   }
   
   [ServerCallback]
   private void OnCollisionEnter(Collision other) {
      var h = other.gameObject.GetComponent<Health>();
      if (h) {
         onHitEffect.HitHealth(h);
      }
      onHitEffect.HitAny(other.gameObject);
      rigidbody.velocity = Vector3.zero;
      rigidbody.angularVelocity = Vector3.zero;
      rigidbody.isKinematic = true;
   }
}