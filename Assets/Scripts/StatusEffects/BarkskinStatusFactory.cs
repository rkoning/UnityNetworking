using System.Collections;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "ScriptableObjects/StatusEffect/BarkskinStatusFactory")]
public class BarkskinStatusFactory: StatusFactory<BarkskinStatusData, BarkskinStatus> {}

[System.Serializable]
public class BarkskinStatusData : StatusData {
   public float armorAdded = 50f;
   public float duration = 20f;
   public GameObject effectPrefab;
}
 
public class BarkskinStatus: Status<BarkskinStatusData> {

   private GameObject effectObject;
   private BaseAvatar targetAvatar;

   public override void Apply() {
      target.StartCoroutine(ApplyOvertime(data.duration));
      targetAvatar = target.GetComponent<BaseAvatar>();
      targetAvatar.maxArmor.bonus += data.armorAdded;
      target.armor += data.armorAdded;

      effectObject = GameObject.Instantiate(data.effectPrefab, target.transform.position + Vector3.up, target.transform.rotation, target.transform);
      effectObject.GetComponent<NetworkTransformChild>().target = target.transform;
      NetworkServer.Spawn(effectObject);
      base.Apply();
   }

   public override void PerTick() { }

   public override void Unapply() {
      base.Unapply();
      targetAvatar.maxArmor.bonus -= data.armorAdded;

      NetworkServer.Destroy(effectObject);
   }

   public IEnumerator UnapplicationCoroutine() {
      yield return new WaitForSeconds(data.duration);
   }
}