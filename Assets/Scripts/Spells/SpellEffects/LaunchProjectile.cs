using UnityEngine;

public class LaunchProjectile : SpellEffect {
   public GameObject projectilePrefab;
   public int maxProjectiles = 3;

   private System.Guid assetId;

   public float castForce;
   private ObjectPool pool;
   private ObjectPool Pool {
      get {
         if (pool == null) {
            pool = ObjectPool.singleton;
         }
         return pool;
      }
   }

   public override void Register(Spell spell) {
      assetId = Pool.RegisterPrefab(projectilePrefab, maxProjectiles);
      base.Register(spell);
   }

   public override void Cast() {
      GameObject projectile = Pool.GetFromPool(assetId, transform.position, transform.rotation);
      var spellProjectile = projectile.GetComponent<SpellProjectile>();
      spellProjectile.onHitEffect = this;
      spellProjectile.rigidbody.AddForce(transform.forward * castForce);
   }
}