using UnityEngine;
using BehaviourTree;

public class TreantController : NPCController
{
    private Node root;

    public override void OnStartServer()
    {
        base.OnStartServer();
        health.OnDamaged += (float damage) => animator.SetTrigger("Take Damage");
        health.OnDeath += (float damage) => animator.SetTrigger("Die");
        InvokeRepeating(nameof(GetRandomPath), 0f, 12f);
    }

    private void GetRandomPath() {
        if (health.IsDead || !agent) {
            return;
        }
        Vector2 randomPoint = Random.insideUnitSphere * 30f;
        agent.SetDestination(transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y));
    }

    private void Update()
    {
        if (!isServer || health.IsDead || !agent) {
            return;
        }

        // root.Evaluate();
        float forwardSpeed = Vector3.Project(agent.desiredVelocity, transform.right).magnitude;
        animator.SetBool("Walk Forward", forwardSpeed > 0);

        // if enemy nearby
            // move to enemy
            // in projectile range?
                // fire projectile when it is available and move to target
            // in melee range?
                // use root attack if it is up
                // otherwise use stomp and bite attacks
        // if controller is out of follow range
            // move to follow range
        // idle
    }

    // public State MoveTo() {

    // }
}
