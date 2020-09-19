using UnityEngine;
using BehaviourTree;

public class PeaShooterController : NPCController {
    private Node root;

    [Header("Attacks")]
    public Spell rangedAttack;
    public float aggroRange;
    public float minAttackRange;
    public float maxAttackRange;
    private Health currentTarget;
    private bool attacking;

    [Header("Follow")]
    public Health followTarget;
    public float followRange;

    public override void OnStartServer()
    {
        base.OnStartServer();
        rangedAttack.Init();
        rangedAttack.OnDone += () => { attacking = false; };
        health.OnDamaged += (float damage) => animator.SetTrigger("Take Damage");
        health.OnDeath += (float damage) => animator.SetTrigger("Die");
        // InvokeRepeating(nameof(GetRandomPath), 0f, 12f);

        root = new Selector(new Node[] {
            // do we have something to follow?
                // check if we are in follow range
                // if not, move to follow range
            new Node(FollowTarget),
            // check if there is an enemy in aggro range
                // are they outside of max attack range?
                    // move to max attack range
                // are they inside min attack range?
                    // move to min attack range
                // otherwise use projectile attack
            new Node(AttackTarget),
            // idle
            new Node(Idle)
        });
    }

    private void GetRandomPath() {
        if (health.IsDead || !agent) {
            return;
        }
        Vector2 randomPoint = Random.insideUnitSphere * 30f;
        agent.SetDestination(transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y));
    }

    protected override void SetAttributes() {
        base.SetAttributes();
        agent.speed = moveSpeed.CurrentValue;
    }

    protected override void Update()
    {
        base.Update();
        if (!isServer || health.IsDead || !agent) {
            return;
        }

        if (attacking) 
            return;
        root.Evaluate();
        
        float forwardSpeed = Vector3.Project(agent.desiredVelocity, transform.right).magnitude;
        animator.SetBool("Walk Forward", forwardSpeed > 0);
    }

    /// <summary>
    /// Base state, always returns success
    /// </summary>
    /// <returns>State.Success</returns>
    private State Idle() => State.Success;

    /// <summary>
    /// Checks if the NPC has a followTarget and if the followTarget is outside their follow range
    /// if so, the NPC's NavMeshAgent will be given a destination within the follow range
    /// </summary>
    /// <returns>State.Success if the followTarget is outside followRange, otherwise State.False</returns>
    private State FollowTarget() {
        if (!followTarget)
            return State.Failure; // Exit if we have nothing to follow
        Vector3 direction = transform.position - followTarget.transform.position;
        
        if (direction.sqrMagnitude < followRange * followRange)
            return State.Failure; // Exit if we are inside the follow range

        Ray dir = new Ray(followTarget.transform.position, direction);
        Vector3 targetPosition = dir.GetPoint(followRange * .8f);
        return State.Success; // Move to the targetPosition if we are outside follow range and have a target
    }

    private State AttackTarget() {
        if (!faction) {
            return State.Failure; // if we don't have a faction, exit
        }

        if (currentTarget) {
            // if we have a target attack it
            Vector3 targetDirection = transform.position - currentTarget.transform.position;
            float sqrDist = targetDirection.sqrMagnitude;
            if (sqrDist > maxAttackRange * maxAttackRange) {
                // move into attack range if we are too far away
                agent.SetDestination(new Ray(currentTarget.transform.position, targetDirection).GetPoint(maxAttackRange));
            } else if (sqrDist < minAttackRange * minAttackRange) {
                // move away from target if too close
            } else {
                attacking = true;
                agent.SetDestination(transform.position);
                rangedAttack.transform.LookAt(currentTarget.transform.position);
                rangedAttack.Cast();
            }
        } else {
            // otherwise request a target and continue
            currentTarget = faction.GetEnemyMember(transform.position, aggroRange);
            return State.Failure;
        }
        return State.Success;
    }
}