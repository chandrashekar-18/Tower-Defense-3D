using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Level
{
    /// <summary>
    /// ScriptableObject for level data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Level", menuName = "Tower Defense/Level Data")]
    public class LevelData : ScriptableObject
    {
        #region Variables
        [SerializeField] private int levelNumber;
        [SerializeField] private string levelName;
        [SerializeField] private int gridWidth = 15;
        [SerializeField] private int gridHeight = 10;
        [SerializeField] private CellTypeData[,] gridData;
        [SerializeField] private List<WaveData> waves = new List<WaveData>();
        [SerializeField] private int startingCurrency = 300;
        #endregion

        #region Properties
        public int LevelNumber { get => levelNumber; set => levelNumber = value; }
        public string LevelName => levelName;
        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public List<WaveData> Waves => waves;
        public int StartingCurrency => startingCurrency;
        #endregion

        #region Public Methods
        public void InitializeDefaultValues()
        {
            levelName = $"Level {levelNumber}";
            gridWidth = 15;
            gridHeight = 10;
            startingCurrency = 300;

            // Initialize grid data
            gridData = new CellTypeData[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    // Default to empty cells
                    gridData[x, z] = new CellTypeData { CellType = Grid.CellType.Empty };
                }
            }

            // Create a simple path
            // CreateDefaultPath();

            // Create default waves
            // CreateDefaultWaves();
        }

        public Grid.CellType GetCellType(int x, int z)
        {
            if (x < 0 || x >= gridWidth || z < 0 || z >= gridHeight || gridData == null)
                return Grid.CellType.Empty;

            return gridData[x, z].CellType;
        }

        public void SetCellType(int x, int z, Grid.CellType cellType)
        {
            if (x < 0 || x >= gridWidth || z < 0 || z >= gridHeight || gridData == null)
                return;

            gridData[x, z].CellType = cellType;
        }

        public void AddWave(WaveData wave)
        {
            waves.Add(wave);
        }

        public void RemoveWave(int index)
        {
            if (index >= 0 && index < waves.Count)
            {
                waves.RemoveAt(index);
            }
        }

        public void ClearWaves()
        {
            waves.Clear();
        }

        public string ToJson()
        {
            LevelDataSerializable serializable = new LevelDataSerializable(this);
            return JsonUtility.ToJson(serializable, true);
        }

        public static LevelData FromJson(string json)
        {
            LevelDataSerializable serializable = JsonUtility.FromJson<LevelDataSerializable>(json);
            return serializable.ToLevelData();
        }
        #endregion

        #region Private Methods
        // private void CreateDefaultPath()
        // {
        //     // Create a simple path from left to right
        //     int pathZ = gridHeight / 2;

        //     // Add spawn point on left
        //     SetCellType(0, pathZ, Grid.CellType.SpawnPoint);

        //     // Add path
        //     for (int x = 1; x < gridWidth - 1; x++)
        //     {
        //         SetCellType(x, pathZ, Grid.CellType.Path);
        //     }

        //     // Add exit point on right
        //     SetCellType(gridWidth - 1, pathZ, Grid.CellType.ExitPoint);
        // }

        // private void CreateDefaultWaves()
        // {
        //     // Create 3 increasingly difficult waves
        //     for (int i = 0; i < 3; i++)
        //     {
        //         WaveData wave = new WaveData();
        //         wave.DelayBetweenGroups = 5f;

        //         // Add enemy groups
        //         int groupCount = 2 + i;
        //         for (int g = 0; g < groupCount; g++)
        //         {
        //             EnemyGroupData group = new EnemyGroupData();
        //             group.EnemyType = (g % 2 == 0) ? Enemies.EnemyType.Basic :
        //                              (i >= 2) ? Enemies.EnemyType.TowerAttacker :
        //                              (i >= 1) ? Enemies.EnemyType.Tank :
        //                              Enemies.EnemyType.Fast;
        //             group.Count = 5 + (i * 3);
        //             group.SpawnDelay = 1f;

        //             wave.EnemyGroups.Add(group);
        //         }

        //         waves.Add(wave);
        //     }
        // }
        #endregion
    }

    /// <summary>
    /// Serializable struct for cell type data.
    /// </summary>
    [System.Serializable]
    public struct CellTypeData
    {
        public Grid.CellType CellType;
    }

    /// <summary>
    /// Serializable class for level data.
    /// </summary>
    [System.Serializable]
    public class LevelDataSerializable
    {
        public int levelNumber;
        public string levelName;
        public int gridWidth;
        public int gridHeight;
        public int[] gridDataFlattened;
        public List<WaveDataSerializable> waves;
        public int startingCurrency;

        public LevelDataSerializable(LevelData levelData)
        {
            levelNumber = levelData.LevelNumber;
            levelName = levelData.LevelName;
            gridWidth = levelData.GridWidth;
            gridHeight = levelData.GridHeight;
            startingCurrency = levelData.StartingCurrency;

            // Flatten grid data
            gridDataFlattened = new int[gridWidth * gridHeight];
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    gridDataFlattened[x + z * gridWidth] = (int)levelData.GetCellType(x, z);
                }
            }

            // Convert waves
            waves = new List<WaveDataSerializable>();
            foreach (WaveData wave in levelData.Waves)
            {
                waves.Add(new WaveDataSerializable(wave));
            }
        }

        public LevelData ToLevelData()
        {
            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();

            // Set basic properties
            levelData.InitializeDefaultValues();

            // Set grid data
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    Grid.CellType cellType = (Grid.CellType)gridDataFlattened[x + z * gridWidth];
                    levelData.SetCellType(x, z, cellType);
                }
            }

            // Set waves
            levelData.ClearWaves();
            foreach (WaveDataSerializable waveSerializable in waves)
            {
                levelData.AddWave(waveSerializable.ToWaveData());
            }

            return levelData;
        }
    }
}