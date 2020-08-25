using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class NPC : NetworkBehaviour {

   private NavMeshAgent agent;

   public override void OnStartServer() {
      agent = GetComponent<NavMeshAgent>();
      InvokeRepeating("GetRandomPath", 0f, 12f);
   }

   private void GetRandomPath() {
      Vector2 randomPoint = Random.insideUnitSphere * 30f;
      agent.SetDestination(new Vector3(randomPoint.x, 0f, randomPoint.y));
   }
}