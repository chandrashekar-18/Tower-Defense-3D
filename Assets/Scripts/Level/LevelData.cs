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

            gridData = new CellTypeData[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    gridData[x, z] = new CellTypeData { CellType = Grid.CellType.Empty };
                }
            }
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

            gridDataFlattened = new int[gridWidth * gridHeight];
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    gridDataFlattened[x + z * gridWidth] = (int)levelData.GetCellType(x, z);
                }
            }

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

            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    Grid.CellType cellType = (Grid.CellType)gridDataFlattened[x + z * gridWidth];
                    levelData.SetCellType(x, z, cellType);
                }
            }

            levelData.ClearWaves();
            foreach (WaveDataSerializable waveSerializable in waves)
            {
                levelData.AddWave(waveSerializable.ToWaveData());
            }

            return levelData;
        }
    }
}