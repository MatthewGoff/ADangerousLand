public struct TerrainType
{
    public readonly TerrainTypeEnum Type;
    public readonly TerrainSubtypeEnum Subtype;

    public TerrainType(TerrainTypeEnum type, TerrainSubtypeEnum subtype)
    {
        Type = type;
        Subtype = subtype;
    }
}