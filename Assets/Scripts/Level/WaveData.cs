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
        [SerializeField] private List<EnemyGroupData> _enemyGroups = new List<EnemyGroupData>();
        [SerializeField] private float _delayBetweenGroups = 3f;

        public List<EnemyGroupData> EnemyGroups => _enemyGroups;
        public float DelayBetweenGroups { get => _delayBetweenGroups; set => _delayBetweenGroups = value; }

        public void AddEnemyGroup(EnemyGroupData group)
        {
            _enemyGroups.Add(group);
        }
    }

    /// <summary>
    /// Data class for enemy groups within a wave
    /// </summary>
    [System.Serializable]
    public class EnemyGroupData
    {
        [SerializeField] private EnemyType _enemyType = EnemyType.Basic;
        [SerializeField] private int _count = 5;
        [SerializeField] private float _spawnDelay = 1f;

        public EnemyType EnemyType { get => _enemyType; set => _enemyType = value; }
        public int Count { get => _count; set => _count = value; }
        public float SpawnDelay { get => _spawnDelay; set => _spawnDelay = value; }
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
        public int enemyType;
        public int count;
        public float spawnDelay;

        public EnemyGroupDataSerializable(EnemyGroupData groupData)
        {
            enemyType = (int)groupData.EnemyType;
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