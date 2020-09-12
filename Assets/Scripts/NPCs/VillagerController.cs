using UnityEngine;

public class VillagerController : NPCController {
   public override void OnStartServer() {
      base.OnStartServer();
      health.OnDeath += (float damage) => {
         animator.SetTrigger("Dying");
         GetComponent<Collider>().enabled = false;
         dead = true;
      };
      agent.updatePosition = false;
      InvokeRepeating(nameof(GetRandomPath), 0f, 12f);
   }

   private void GetRandomPath() {
      if (dead) {
         return;
      }
      Vector2 randomPoint = Random.insideUnitSphere * 30f;
      agent.SetDestination(transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y));
   }

   private void Update() {
      if (!isServer || dead || !agent) {
         return;
      }
      Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

      // Map 'worldDeltaPosition' to local space
      float dx = Vector3.Dot (transform.right, worldDeltaPosition);
      float dy = Vector3.Dot (transform.forward, worldDeltaPosition);
      Vector2 deltaPosition = new Vector2 (dx, dy);

      // Low-pass filter the deltaMove
      float smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
      smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

      // Update velocity if time advances
      if (Time.deltaTime > 1e-5f)
         velocity = smoothDeltaPosition / Time.deltaTime;

      bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

      // Update animation parameters
      animator.SetBool("move", shouldMove);
      animator.SetFloat ("velx", velocity.x);
      animator.SetFloat ("vely", velocity.y);
   }
}