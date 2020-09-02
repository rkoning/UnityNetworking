using System.Collections;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "ScriptableObjects/StatusEffect/BurningStatusFactory")]
public class BurningStatusFactory: StatusFactory<BurningStatusData, BurningStatus> {}

[System.Serializable]
public class BurningStatusData {
   public float damagePerTick = 1f;
   public float duration = 5f;
   public GameObject effectPrefab;
}

public class BurningStatus: Status<BurningStatusData> {

   private GameObject effectObject;

   public override void Apply() {
      Debug.Log("Applied!");
      target.StartCoroutine(ApplyOvertime(data.duration));
      effectObject = GameObject.Instantiate(data.effectPrefab, target.transform.position + Vector3.up, target.transform.rotation, target.transform);
      NetworkServer.Spawn(effectObject);
   }

   public override void PerTick() {
      target.TakeDamage(data.damagePerTick, source); 
   }

   public override void Unapply() {
      NetworkServer.Destroy(effectObject);
   }

   public IEnumerator UnapplicationCoroutine() {
      yield return new WaitForSeconds(data.duration);
   }
}