using UnityEngine;
using TowerDefense.Enemies;
using System.Collections.Generic;

namespace TowerDefense.Level
{
    /// <summary>
    /// Data class for enemy waves
    /// </summary>
    [System.Serializable]
    public class WaveData
    {
        [SerializeField] private List<EnemyGroupData> enemyGroups = new List<EnemyGroupData>();
        [SerializeField] private float delayBetweenGroups = 3f;

        public List<EnemyGroupData> EnemyGroups => enemyGroups;
        public float DelayBetweenGroups => delayBetweenGroups;

        public void AddEnemyGroup(EnemyGroupData group)
        {
            enemyGroups.Add(group);
        }

        public int GetTotalEnemyCount()
        {
            int total = 0;
            foreach (var group in enemyGroups)
            {
                total += group.Count;
            }
            return total;
        }
    }

    /// <summary>
    /// Data class for enemy groups within a wave
    /// </summary>
    [System.Serializable]
    public class EnemyGroupData
    {
        [SerializeField] private string enemyId;
        [SerializeField] private int count = 5;
        [SerializeField] private float spawnDelay = 1f;

        public string EnemyId => enemyId;
        public int Count => count;
        public float SpawnDelay => spawnDelay;
    }

    /// <summary>
    /// Serializable class for wave data
    /// </summary>
    [System.Serializable]
    public class WaveDataSerializable
    {
        public List<EnemyGroupDataSerializable> enemyGroups;
        public float delayBetweenGroups;

        public WaveDataSerializable(WaveData waveData)
        {
            delayBetweenGroups = waveData.DelayBetweenGroups;

            enemyGroups = new List<EnemyGroupDataSerializable>();
            foreach (EnemyGroupData groupData in waveData.EnemyGroups)
            {
                enemyGroups.Add(new EnemyGroupDataSerializable(groupData));
            }
        }

        public WaveData ToWaveData()
        {
            WaveData waveData = new WaveData();

            foreach (EnemyGroupDataSerializable groupSerializable in enemyGroups)
            {
                waveData.AddEnemyGroup(groupSerializable.ToEnemyGroupData());
            }

            return waveData;
        }
    }

    /// <summary>
    /// Serializable class for enemy group data
    /// </summary>
    [System.Serializable]
    public class EnemyGroupDataSerializable
    {
        public string enemyId;
        public int count;
        public float spawnDelay;

        public EnemyGroupDataSerializable(EnemyGroupData groupData)
        {
            enemyId = groupData.EnemyId;
            count = groupData.Count;
            spawnDelay = groupData.SpawnDelay;
        }

        public EnemyGroupData ToEnemyGroupData()
        {
            EnemyGroupData groupData = new EnemyGroupData();
            return groupData;
        }
    }
}