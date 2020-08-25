using UnityEngine;
using Mirror;

public class LaunchProjectile : SpellEffect {
   public GameObject projectilePrefab;
   public int maxProjectiles = 3;


   public float castForce;

   public override void Register(Spell spell) {
      ObjectPool.singleton.RegisterPrefab(projectilePrefab.name, maxProjectiles);
      base.Register(spell);
   }

   public override void Cast() {
      CmdSpawnProjectile();
   }

   [Command]
   private void CmdSpawnProjectile() {
      var projectile = ObjectPool.singleton.GetFromPool(projectilePrefab.name, transform.position, transform.rotation);
      
      var spellProjectile = projectile.GetComponent<SpellProjectile>();
      spellProjectile.onHitEffect = this;
      spellProjectile.rigidbody.AddForce(transform.forward * castForce);
   }

}