using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using ProtoBuf;

public class WorldPersistenceManager
{
    private static readonly string PATH = "A Dangerous Land_Data/data/world/";
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

    public static World LoadWorld(int worldIdentifier)
    {
        GameManager.Singleton.Print("Starting to load world at " + DateTime.Now.ToString("h:mm:ss tt"));
        World world;
        Stream file = File.OpenRead(PATH + worldIdentifier.ToString() + ".bin");
        world = Serializer.Deserialize<World>(file);
        GameManager.Singleton.Print("Finished laoding world at " + DateTime.Now.ToString("h:mm:ss tt"));
        return world;
    }

    public static World LoadWorldBinaryFormatter(int worldIdentifier)
    {
        GameManager.Singleton.Print("Starting to load world at " + DateTime.Now.ToString("h:mm:ss tt"));
        World world;
        Stream openFileStream = File.OpenRead(PATH + worldIdentifier.ToString() + ".bin");
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        world = (World)binaryFormatter.Deserialize(openFileStream);
        openFileStream.Close();
        GameManager.Singleton.Print("Finished laoding world at " + DateTime.Now.ToString("h:mm:ss tt"));
        return world;
    }

    public static void SaveWorld(World world)
    {
        GameManager.Singleton.Print("Starting to save world at " + DateTime.Now.ToString("h:mm:ss tt"));
        Stream file = File.Create(PATH + world.WorldIdentifier.ToString() + ".bin");
        Serializer.Serialize(file, world);
        GameManager.Singleton.Print("Finished saving world at "+ DateTime.Now.ToString("h:mm:ss tt"));

    }

    public static void SaveWorldBinaryFormatter(World world)
    {
        GameManager.Singleton.Print("Starting to save world at " + DateTime.Now.ToString("h:mm:ss tt"));
        Stream saveFileStream = File.Create(PATH + world.WorldIdentifier.ToString() + ".bin");
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(saveFileStream, world);
        saveFileStream.Close();
        GameManager.Singleton.Print("Finished saving world at " + DateTime.Now.ToString("h:mm:ss tt"));

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
            Version = "alpha 0.5",
            Name = name,
            Seed = seed,
        };
        MetaData.Add(newMetaData);
        SaveMetaData();

        World newWorld = new World(newMetaData.WorldIdentifier, newMetaData.Seed);
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
