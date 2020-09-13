public class SetTargetPosition : SpellEffect {
    public override void HitHealth(Health other) {
        other.transform.position = transform.position;
    }
}