using UnityEngine;

public enum CompanionRace
{
    Human,
    Elf,
    Dwarf,
    Fairy,
    Orc,
    Android,
    Demon,
    Angel,
    Mermaid
}

[CreateAssetMenu(menuName = "Combo Blocks/Companions/Companion Definition")]
public class CompanionDefinition : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string displayName;
    public CompanionRace race;
    public string role;

    [TextArea(2, 4)]
    public string description;

    [Header("Unlock")]
    public int unlockLevel;
    public string linkedPowerUpId;

    [Header("Local Placeholder")]
    public Sprite placeholderPortrait;

    [Header("Future Addressables Keys")]
    public string cardArtKey;
    public string portraitArtKey;
    public string fullBodyArtKey;

    [Header("Affinity Outfits")]
    public CompanionOutfitDefinition[] outfits;
}

[System.Serializable]
public class CompanionOutfitDefinition
{
    public string outfitId;
    public string displayName;

    [TextArea(2, 4)]
    public string description;

    public int requiredAffinityLevel;

    public Sprite localPreview;
    public string portraitAddressKey;
    public string fullBodyAddressKey;
}