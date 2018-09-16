using System;

[Serializable]
public struct PlayerPersistenceMetaData
{
    public int PlayerIdentifier;
    public string Version;
    public string Name;
    public int Level;
    public DeathPenaltyType DeathPenalty;
}