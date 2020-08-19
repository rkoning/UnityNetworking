using System.Net.NetworkInformation;
using UnityEngine;

public class ApplyToGround : SpellEffect {
   
   public override void Cast() {
      Ray ray = new Ray(transform.position, Vector3.down);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit, 10f)) {
         transform.position = hit.point;
         transform.rotation = Quaternion.identity;
         base.Cast();
      }
   }
}