using System.Collections;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "ScriptableObjects/StatusEffect/BarkskinStatusFactory")]
public class BarkskinStatusFactory: StatusFactory<BarkskinStatusData, BarkskinStatus> {}

[System.Serializable]
public class BarkskinStatusData {
   public float armorAdded = 50f;
   public float duration = 20f;
   public GameObject effectPrefab;
}
 
public class BarkskinStatus: Status<BarkskinStatusData> {

   private GameObject effectObject;
   private Avatar targetPlayer;

   public override void Apply() {
      target.StartCoroutine(ApplyOvertime(data.duration));
      targetPlayer = target.GetComponent<Avatar>();
      if (targetPlayer)
         targetPlayer.maxArmor.bonus += data.armorAdded;
      else 
         target.maxArmor += data.armorAdded;
      target.armor += data.armorAdded;
      // target.OnArmorBroken.
      effectObject = GameObject.Instantiate(data.effectPrefab, target.transform.position + Vector3.up, target.transform.rotation, target.transform);
      NetworkServer.Spawn(effectObject);
      base.Apply();
   }

   public override void PerTick() { }

   public override void Unapply() {
      base.Unapply();
      if (targetPlayer)
         targetPlayer.maxArmor.bonus -= data.armorAdded;
      else 
         target.maxArmor -= data.armorAdded;
      NetworkServer.Destroy(effectObject);
   }

   public IEnumerator UnapplicationCoroutine() {
      yield return new WaitForSeconds(data.duration);
   }
}