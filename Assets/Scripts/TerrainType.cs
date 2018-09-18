using MessagePack;

[MessagePackObject]
public struct TerrainType
{
    [Key(0)] public readonly TerrainTypeEnum Type;
    [Key(1)] public readonly TerrainSubtypeEnum Subtype;

    [SerializationConstructor]
    public TerrainType(TerrainTypeEnum type, TerrainSubtypeEnum subtype)
    {
        Type = type;
        Subtype = subtype;
    }
}