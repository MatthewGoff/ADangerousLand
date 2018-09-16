﻿using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerPersistenceManager
{
    private static readonly string PATH = "A Dangerous Land_Data/data/player/";
    private static readonly string META_DATA_FILE_NAME = "playermetadata.bin";
    private static List<PlayerPersistenceMetaData> MetaData;

    public static void Initialize()
    {
        if (File.Exists(PATH + META_DATA_FILE_NAME))
        {
            LoadMetaData();   
        }
        else
        {
            MetaData = new List<PlayerPersistenceMetaData>();
            SaveMetaData();
        }        
    }

    public static PlayerManager LoadPlayer(int playerIdentifier)
    {
        PlayerManager playerManager;
        Stream openFileStream = File.OpenRead(PATH + playerIdentifier.ToString() + ".bin");
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        playerManager = (PlayerManager)binaryFormatter.Deserialize(openFileStream);
        openFileStream.Close();
        return playerManager;
    }

    public static void SavePlayer(PlayerManager playerManager)
    {
        Stream saveFileStream = File.Create(PATH + playerManager.PlayerIdentifier.ToString() + ".bin");
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(saveFileStream, playerManager);
        saveFileStream.Close();
    }

    public static void DeletePlayer(int playerIdentifier)
    {
        File.Delete(PATH + playerIdentifier.ToString() + ".bin");
        PlayerPersistenceMetaData toDelete = default;
        foreach (PlayerPersistenceMetaData metaData in MetaData)
        {
            if (metaData.PlayerIdentifier == playerIdentifier)
            {
                toDelete = metaData;

            }
        }
        MetaData.Remove(toDelete);
        SaveMetaData();
    }

    public static int CreatePlayer(string name, DeathPenaltyType deathPenalty)
    {
        PlayerPersistenceMetaData newMetaData = new PlayerPersistenceMetaData
        {
            PlayerIdentifier = NextAvailableIdentifier(),
            Version = "alpha 0.5",
            Name = name,
            Level = 1,
            DeathPenalty = deathPenalty,
        };
        MetaData.Add(newMetaData);
        SaveMetaData();

        PlayerManager newPlayer = new PlayerManager(newMetaData.PlayerIdentifier, name, deathPenalty);
        SavePlayer(newPlayer);
        return newPlayer.PlayerIdentifier;
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
        MetaData = (List<PlayerPersistenceMetaData>)binaryFormatter.Deserialize(openFileStream);
        openFileStream.Close();
    }

    public static List<int> GetPlayerIdentifiers()
    {
        List<int> returnList = new List<int>();
        foreach (PlayerPersistenceMetaData metaData in MetaData)
        {
            returnList.Add(metaData.PlayerIdentifier);
        }
        return returnList;
    }

    public static int NextAvailableIdentifier()
    {
        for (int i=0; i<int.MaxValue; i++)
        {
            if (!IdentifierInUse(i))
            {
                return i;
            }
        }
        return 0;
    }

    public static bool IdentifierInUse(int playerIdentifier)
    {
        foreach (PlayerPersistenceMetaData metaData in MetaData)
        {
            if (metaData.PlayerIdentifier == playerIdentifier)
            {
                return true;
            }
        }
        return false;
    }

    public static PlayerPersistenceMetaData GetPlayerPersistenceMetaData(int playerIdentifier)
    {
        foreach (PlayerPersistenceMetaData metaData in MetaData)
        {
            if (metaData.PlayerIdentifier == playerIdentifier)
            {
                return metaData;
            }
        }

        return default;
    }
}
