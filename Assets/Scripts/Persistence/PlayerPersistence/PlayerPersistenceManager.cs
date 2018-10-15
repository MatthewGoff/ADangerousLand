using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;
using UnityEngine;
using ADL.Core;
using ADL.Combat.Player;

namespace ADL.Persistence
{
    public static class PlayerPersistenceManager
    {
        private static readonly string PATH = Application.persistentDataPath + "/data/player/";
        private static readonly string META_DATA_FILE_NAME = "playermetadata.bin";
        private static List<PlayerPersistenceMetaData> MetaData;

        public static void Initialize()
        {
            if (File.Exists(PATH + META_DATA_FILE_NAME))
            {
                Core.Logger.LogInfo("Found existing player meta data. Loading...");
                LoadMetaData();
            }
            else
            {
                Core.Logger.LogInfo("Failed to find existing player meta data. Creating...");
                MetaData = new List<PlayerPersistenceMetaData>();
                SaveMetaData();
            }
        }

        public static PlayerManager LoadPlayer(int playerIdentifier)
        {
            byte[] bytes = File.ReadAllBytes(PATH + playerIdentifier.ToString() + ".bin");
            PlayerManager playerManager = MessagePackSerializer.Deserialize<PlayerManager>(bytes);
            return playerManager;
        }

        public static void SavePlayer(PlayerManager playerManager)
        {
            byte[] bytes = MessagePackSerializer.Serialize(playerManager);
            File.WriteAllBytes(PATH + playerManager.PlayerIdentifier.ToString() + ".bin", bytes);

            PlayerPersistenceMetaData metaData = GetPlayerPersistenceMetaData(playerManager.PlayerIdentifier);
            metaData.Level = playerManager.Level;
            SaveMetaData();
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

        public static int CreatePlayer(string name, float color, DeathPenaltyType deathPenalty)
        {
            PlayerPersistenceMetaData newMetaData = new PlayerPersistenceMetaData
            {
                PlayerIdentifier = NextAvailableIdentifier(),
                Version = "alpha 0.7",
                Name = name,
                Level = 1,
                DeathPenalty = deathPenalty,
                Color = color,
            };
            MetaData.Add(newMetaData);
            SaveMetaData();

            PlayerManager newPlayer = new PlayerManager(newMetaData.PlayerIdentifier, newMetaData.Name, newMetaData.Color, newMetaData.DeathPenalty);
            SavePlayer(newPlayer);
            return newPlayer.PlayerIdentifier;
        }

        public static bool SaveMetaData()
        {
            try
            {
                Directory.CreateDirectory(PATH);
            }
            catch (Exception e)
            {
                string log = "Failed to create directory: " + PATH;
                log += "\n" + e.ToString();
                Core.Logger.LogException(log);
                return false;
            }
            Stream saveFileStream = null;
            try
            {
                saveFileStream = File.Create(PATH + META_DATA_FILE_NAME);
            }
            catch (Exception e)
            {
                string log = "Failed to create file: " + PATH + META_DATA_FILE_NAME;
                log += "\n" + e.ToString();
                Core.Logger.LogException(log);
                return false;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                binaryFormatter.Serialize(saveFileStream, MetaData);
            }
            catch (Exception e)
            {
                string log = "Failed to write meta data to file";
                log += "\n" + e.ToString();
                Core.Logger.LogException(log);
                return false;
            }
            try
            {
                saveFileStream.Close();
            }
            catch (Exception e)
            {
                string log = "Failed to close file";
                log += "\n" + e.ToString();
                Core.Logger.LogException(log);
                return false;
            }
            return true;
        }

        public static bool LoadMetaData()
        {
            Stream openFileStream = null;
            try
            {
                openFileStream = File.OpenRead(PATH + META_DATA_FILE_NAME);
            }
            catch (Exception e)
            {
                string log = "Failed to open file: " + PATH + META_DATA_FILE_NAME;
                log += "\n" + e.ToString();
                Core.Logger.LogException(log);
                return false;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                MetaData = (List<PlayerPersistenceMetaData>)binaryFormatter.Deserialize(openFileStream);
            }
            catch (Exception e)
            {
                string log = "Failed to read meta data from file";
                log += "\n" + e.ToString();
                Core.Logger.LogException(log);
                return false;
            }
            try
            {
                openFileStream.Close();
            }
            catch (Exception e)
            {
                string log = "Failed to close file";
                log += "\n" + e.ToString();
                Core.Logger.LogException(log);
                return false;
            }
            return true;
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
            for (int i = 0; i < int.MaxValue; i++)
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
}