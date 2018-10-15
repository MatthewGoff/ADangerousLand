using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;
using UnityEngine;
using ADL.World;

namespace ADL.Persistence
{
    public static class WorldPersistenceManager
    {
        private static readonly string PATH = Application.persistentDataPath + "/data/world/";
        private static readonly string META_DATA_FILE_NAME = "worldmetadata.bin";
        private static List<WorldPersistenceMetaData> MetaData;

        public static void Initialize()
        {
            if (File.Exists(PATH + META_DATA_FILE_NAME))
            {
                LoadMetaData();
            }
            else
            {
                MetaData = new List<WorldPersistenceMetaData>();
                SaveMetaData();
            }
        }

        public static WorldManager LoadWorld(int worldIdentifier)
        {
            byte[] bytes = File.ReadAllBytes(PATH + worldIdentifier.ToString() + ".bin");
            WorldManager world = MessagePackSerializer.Deserialize<WorldManager>(bytes);
            return world;
        }

        public static void SaveWorld(WorldManager world)
        {
            byte[] bytes = MessagePackSerializer.Serialize(world);
            File.WriteAllBytes(PATH + world.WorldIdentifier.ToString() + ".bin", bytes);
        }

        public static void DeleteWorld(int worldIdentifier)
        {
            File.Delete(PATH + worldIdentifier.ToString() + ".bin");
            WorldPersistenceMetaData toDelete = default;
            foreach (WorldPersistenceMetaData metaData in MetaData)
            {
                if (metaData.WorldIdentifier == worldIdentifier)
                {
                    toDelete = metaData;

                }
            }
            MetaData.Remove(toDelete);
            SaveMetaData();
        }

        public static int CreateWorld(string name, int seed)
        {
            WorldPersistenceMetaData newMetaData = new WorldPersistenceMetaData
            {
                WorldIdentifier = NextAvailableIdentifier(),
                Version = "alpha 0.7",
                Name = name,
                Seed = seed,
            };
            MetaData.Add(newMetaData);
            SaveMetaData();

            WorldManager newWorld = new WorldManager(newMetaData.WorldIdentifier, newMetaData.Seed);
            SaveWorld(newWorld);
            return newWorld.WorldIdentifier;
        }

        public static void SaveMetaData()
        {
            Directory.CreateDirectory(PATH);
            Stream saveFileStream = File.Create(PATH + META_DATA_FILE_NAME);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(saveFileStream, MetaData);
            saveFileStream.Close();
        }

        public static void LoadMetaData()
        {
            Stream openFileStream = File.OpenRead(PATH + META_DATA_FILE_NAME);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MetaData = (List<WorldPersistenceMetaData>)binaryFormatter.Deserialize(openFileStream);
            openFileStream.Close();
        }

        public static List<int> GetWorldIdentifiers()
        {
            List<int> returnList = new List<int>();
            foreach (WorldPersistenceMetaData metaData in MetaData)
            {
                returnList.Add(metaData.WorldIdentifier);
            }
            return returnList;
        }

        public static int NextAvailableIdentifier()
        {
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (!IdentifierInUse(i))
                {
                    return i;
                }
            }
            return 0;
        }

        public static bool IdentifierInUse(int worldIdentifier)
        {
            foreach (WorldPersistenceMetaData metaData in MetaData)
            {
                if (metaData.WorldIdentifier == worldIdentifier)
                {
                    return true;
                }
            }
            return false;
        }

        public static WorldPersistenceMetaData GetWorldPersistenceMetaData(int worldIdentifier)
        {
            foreach (WorldPersistenceMetaData metaData in MetaData)
            {
                if (metaData.WorldIdentifier == worldIdentifier)
                {
                    return metaData;
                }
            }

            return default;
        }
    }
}