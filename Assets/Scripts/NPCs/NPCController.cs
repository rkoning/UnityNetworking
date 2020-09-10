using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class NPCController : NetworkBehaviour, IPoolableObject {

   protected NavMeshAgent agent;
   protected Health health;
   protected Animator animator;

   protected Vector2 smoothDeltaPosition = Vector2.zero;
   protected Vector2 velocity = Vector2.zero;
   protected bool dead;

   public void Init() {

   }

   
   public void RpcInit() {
      
   }

   private void OnEnable() {
      if (agent)
         return;
      NavMeshHit closestHit;
      if (NavMesh.SamplePosition(transform.position, out closestHit, 500f, 1)) {
         transform.position = closestHit.position;
         agent = gameObject.GetComponent<NavMeshAgent>();
         agent.baseOffset = 0f;
      }
   }

   public override void OnStartServer() {
      // if (enabled) {
      //    NavMeshHit closestHit;
      //    if (NavMesh.SamplePosition(transform.position, out closestHit, 500f, 1)) {
      //       transform.position = closestHit.position;
      //       agent = gameObject.AddComponent<NavMeshAgent>();
      //    }
      // }
      animator = GetComponent<Animator>();
      health = GetComponent<Health>();
   }

}