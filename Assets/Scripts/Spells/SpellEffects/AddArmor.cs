public class AddArmor : SpellEffect {

    public float bonus = 1f;
    public float multiplier = 0f;

    public override void Cast()
    {
        spell.owner.maxArmor.bonus += bonus;
        spell.owner.maxArmor.multiplier += multiplier;
        
        spell.owner.health.armor += bonus;
    }
}