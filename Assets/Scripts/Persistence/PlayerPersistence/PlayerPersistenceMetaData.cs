using System;
using ADL.Core;

namespace ADL.Persistence
{
    [Serializable]
    public class PlayerPersistenceMetaData
    {
        public int PlayerIdentifier;
        public string Version;
        public string Name;
        public int Level;
        public DeathPenaltyType DeathPenalty;
        public float Color;
    }
}