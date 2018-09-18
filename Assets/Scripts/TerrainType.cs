using ProtoBuf;

[ProtoContract]
public struct TerrainType
{
    [ProtoMember(1)] public readonly TerrainTypeEnum Type;
    [ProtoMember(2)] public readonly TerrainSubtypeEnum Subtype;

    public TerrainType(TerrainTypeEnum type, TerrainSubtypeEnum subtype)
    {
        Type = type;
        Subtype = subtype;
    }
}