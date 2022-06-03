
public enum CharacterStrenthType
{
    DmgGain,
    ArmorPenetration,
    MagicPenetration
}


[System.Serializable]
public class CharacterStrengthAttribute
{
    public CharacterStrenthType type;
    public float magnitude;
    public int everyLevelIncrease;
}
