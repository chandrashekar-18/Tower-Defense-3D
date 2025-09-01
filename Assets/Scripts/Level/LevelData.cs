using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Level
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Tower Defense/Level Data")]
    public class LevelData : ScriptableObject
    {
        #region Serializable Classes
        [System.Serializable]
        private class SerializableGrid
        {
            public List<CellTypeData> cells = new List<CellTypeData>();
            public int width;
            public int height;
        }
        #endregion

        #region Variables
        [SerializeField] private int levelNumber;
        [SerializeField] private string levelName;
        [SerializeField] private int gridWidth = 15;
        [SerializeField] private int gridHeight = 10;
        [SerializeField] private SerializableGrid serializedGrid = new SerializableGrid();
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

        #region Unity Methods
        private void OnValidate()
        {
            if (serializedGrid.cells.Count == 0)
            {
                InitializeDefaultValues();
            }
        }
        #endregion

        #region Public Methods
        public void InitializeDefaultValues()
        {
            levelName = $"Level {levelNumber}";

            // Don't reset grid dimensions if they're already set
            if (serializedGrid.cells.Count == 0)
            {
                gridWidth = 15;
                gridHeight = 10;
            }

            startingCurrency = 300;

            // Initialize the grid
            serializedGrid.width = gridWidth;
            serializedGrid.height = gridHeight;
            serializedGrid.cells.Clear();

            // Initialize all cells as empty
            for (int z = 0; z < gridHeight; z++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    serializedGrid.cells.Add(new CellTypeData { CellType = Grid.CellType.Empty });
                }
            }
        }

        public void ResizeGrid(int newWidth, int newHeight)
        {
            List<CellTypeData> newCells = new List<CellTypeData>();

            // Copy existing data where possible
            for (int z = 0; z < newHeight; z++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    if (x < gridWidth && z < gridHeight && serializedGrid.cells.Count > 0)
                    {
                        newCells.Add(serializedGrid.cells[x + z * gridWidth]);
                    }
                    else
                    {
                        newCells.Add(new CellTypeData { CellType = Grid.CellType.Empty });
                    }
                }
            }

            gridWidth = newWidth;
            gridHeight = newHeight;
            serializedGrid.width = newWidth;
            serializedGrid.height = newHeight;
            serializedGrid.cells = newCells;
        }

        public Grid.CellType GetCellType(int x, int z)
        {
            if (x < 0 || x >= gridWidth || z < 0 || z >= gridHeight ||
                serializedGrid.cells.Count == 0)
                return Grid.CellType.Empty;

            return serializedGrid.cells[x + z * gridWidth].CellType;
        }

        public void SetCellType(int x, int z, Grid.CellType cellType)
        {
            if (x < 0 || x >= gridWidth || z < 0 || z >= gridHeight)
                return;

            if (serializedGrid.cells.Count == 0)
            {
                Debug.LogWarning("Grid data was null, initializing...");
                InitializeDefaultValues();
            }

            // Create a new CellTypeData instance
            CellTypeData newCell = new CellTypeData { CellType = cellType };

            // Replace the entire struct in the list
            int index = x + z * gridWidth;
            serializedGrid.cells[index] = newCell;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
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

        public void DebugPrintGrid()
        {
            Debug.Log($"Grid dimensions: {gridWidth}x{gridHeight}");
            string gridString = "Grid Contents:\n";
            for (int z = 0; z < gridHeight; z++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    gridString += $"{GetCellType(x, z)} ";
                }
                gridString += "\n";
            }
            Debug.Log(gridString);
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

    [System.Serializable]
    public struct CellTypeData
    {
        public Grid.CellType CellType;
    }

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
            levelData.InitializeDefaultValues();

            levelData.LevelNumber = levelNumber;
            levelData.LevelName = levelName;
            levelData.StartingCurrency = startingCurrency;
            levelData.name = levelName;

            // Resize and restore grid data
            levelData.ResizeGrid(gridWidth, gridHeight);

            if (gridDataFlattened != null && gridDataFlattened.Length == gridWidth * gridHeight)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    for (int z = 0; z < gridHeight; z++)
                    {
                        Grid.CellType cellType = (Grid.CellType)gridDataFlattened[x + z * gridWidth];
                        levelData.SetCellType(x, z, cellType);
                    }
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