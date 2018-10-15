using System;

namespace ADL.Persistence
{
    [Serializable]
    public class WorldPersistenceMetaData
    {
        public int WorldIdentifier;
        public string Version;
        public string Name;
        public int Seed;
    }
}