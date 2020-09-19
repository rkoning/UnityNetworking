using UnityEngine;

public class Summon : SpellEffect {
   public GameObject prefab;
   public int maxActiveSummons = 1;

   public override void Register(Spell spell) {
      base.Register(spell);
      ObjectPool.RegisterPrefab(prefab.name, maxActiveSummons);
   }

   public override void Cast() {
      spell.owner.GetFromPool(prefab.name, transform.position, transform.rotation);
   }
}