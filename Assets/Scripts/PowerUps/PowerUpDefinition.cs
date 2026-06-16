using UnityEngine;

public enum PowerUpType
{
    FillSingleCell,
    ClearRow,
    ClearColumn,
    SwapPiece,
    UndoMove,
    BombArea
}

[CreateAssetMenu(menuName = "Combo Blocks/PowerUps/Power Up Definition")]
public class PowerUpDefinition : ScriptableObject
{
    public string id;
    public string displayName;
    public PowerUpType type;

    [TextArea(2, 4)]
    public string description;

    public Sprite localIcon;

    [Header("Future Addressables Keys")]
    public string iconAddressKey;
    public string vfxAddressKey;
}