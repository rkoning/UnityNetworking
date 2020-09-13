[System.Serializable]
public class CharacterAttribute {
    public float baseValue;
    public float bonus;
    public float multiplier;

    public float CurrentValue {
        get { return (baseValue + bonus) * multiplier; }
    }
}