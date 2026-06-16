using UnityEngine;

public enum LevelRewardType
{
    Companion,
    PowerUp,
    Outfit,
    Currency,
    StoryChapter
}

[CreateAssetMenu(menuName = "Combo Blocks/Progression/Level Reward Definition")]
public class LevelRewardDefinition : ScriptableObject
{
    public int level;
    public LevelRewardType rewardType;

    public string rewardId;

    public string title;

    [TextArea(2, 4)]
    public string description;

    public Sprite localPreview;

    [Header("Future Addressables Key")]
    public string previewAddressKey;
}