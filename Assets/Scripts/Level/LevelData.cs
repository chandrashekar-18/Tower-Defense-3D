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
        public string LevelName { get => levelName; set => levelName = value; }
        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public List<WaveData> Waves => waves;
        public int StartingCurrency { get => startingCurrency; set => startingCurrency = value; }
        #endregion

        #region Public Methods
        public void InitializeDefaultValues()
        {
            levelName = $"Level {levelNumber}";
            
            // Don't reset grid dimensions if they're already set
            if (gridData == null)
            {
                gridWidth = 15;
                gridHeight = 10;
            }
            
            startingCurrency = 300;

            // Create new grid with current dimensions
            CellTypeData[,] newGrid = new CellTypeData[gridWidth, gridHeight];

            // Initialize all cells as empty
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    newGrid[x, z] = new CellTypeData { CellType = Grid.CellType.Empty };
                }
            }

            gridData = newGrid;
        }

        // Add this new method for resizing the grid
        public void ResizeGrid(int newWidth, int newHeight)
        {
            CellTypeData[,] newGrid = new CellTypeData[newWidth, newHeight];

            // Copy existing data where possible
            for (int x = 0; x < newWidth; x++)
            {
                for (int z = 0; z < newHeight; z++)
                {
                    if (x < gridWidth && z < gridHeight && gridData != null)
                    {
                        newGrid[x, z] = gridData[x, z];
                    }
                    else
                    {
                        newGrid[x, z] = new CellTypeData { CellType = Grid.CellType.Empty };
                    }
                }
            }

            gridWidth = newWidth;
            gridHeight = newHeight;
            gridData = newGrid;
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

            // Serialize grid data
            gridDataFlattened = new int[gridWidth * gridHeight];
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    gridDataFlattened[x + z * gridWidth] = (int)levelData.GetCellType(x, z);
                }
            }

            // Serialize waves
            waves = new List<WaveDataSerializable>();
            foreach (WaveData wave in levelData.Waves)
            {
                waves.Add(new WaveDataSerializable(wave));
            }
        }

        public LevelData ToLevelData()
        {
            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();

            // Set basic properties using the public setters
            levelData.LevelNumber = levelNumber;
            levelData.LevelName = levelName;
            levelData.StartingCurrency = startingCurrency;

            // Set the name property for the ScriptableObject
            levelData.name = levelName;

            // Resize grid to match loaded dimensions
            levelData.ResizeGrid(gridWidth, gridHeight);

            // Restore grid data
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    Grid.CellType cellType = (Grid.CellType)gridDataFlattened[x + z * gridWidth];
                    levelData.SetCellType(x, z, cellType);
                }
            }

            // Restore waves
            levelData.ClearWaves();
            foreach (WaveDataSerializable waveSerializable in waves)
            {
                levelData.AddWave(waveSerializable.ToWaveData());
            }

            return levelData;
        }
    }
}