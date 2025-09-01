using UnityEngine;
using UnityEditor;
using TowerDefense.Grid;
using TowerDefense.Level;
using System.Collections.Generic;
using System.IO;
using TowerDefense.Enemies;
using System.Linq;

namespace TowerDefense.Editor
{
    public class RandomLevelGenerator : EditorWindow
    {
        private List<EnemyData> enemyData = new List<EnemyData>();
        private string savePath = "Assets/Resources/Levels/";
        private bool enemyDataLoaded = false;

        [MenuItem("Tools/Tower Defense/Generate Random Levels")]
        public static void ShowWindow()
        {
            GetWindow<RandomLevelGenerator>("Random Level Generator");
        }

        private void OnEnable()
        {
            LoadEnemyData();
        }

        private void OnGUI()
        {
            GUILayout.Label("Random Level Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (enemyData.Count == 0)
            {
                EditorGUILayout.HelpBox("No enemy data found! Please create enemy data in Resources/Enemies folder.", MessageType.Warning);
                if (GUILayout.Button("Refresh Enemy Data"))
                {
                    LoadEnemyData();
                }
                return;
            }

            EditorGUILayout.HelpBox(
                $"Found {enemyData.Count} enemy types.\nThis will generate 5 levels with increasing difficulty.",
                MessageType.Info);

            if (GUILayout.Button("Generate Random Levels"))
            {
                GenerateRandomLevels();
            }
        }

        private void LoadEnemyData()
        {
            if (enemyDataLoaded) return;

            enemyData.Clear();
            EnemyData[] enemyAssets = Resources.LoadAll<EnemyData>("Enemies");

            if (enemyAssets == null || enemyAssets.Length == 0)
            {
                Debug.LogError("No enemy data found in Resources/Enemies folder! Please ensure enemy data exists.");
                return;
            }

            enemyData.AddRange(enemyAssets);
            enemyDataLoaded = true;

            string enemyNames = string.Join(", ", enemyData.Select(e => e.EnemyName));
            Debug.Log($"Loaded {enemyData.Count} enemy types: {enemyNames}");
        }

        private void GenerateRandomLevels()
        {
            if (enemyData.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No enemy data found! Cannot generate levels.", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog("Generate Random Levels",
                "This will create 5 random levels in Assets/Resources/Levels/. Continue?", "Yes", "No"))
                return;

            // Ensure directory exists
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            CreateTutorialLevel();
            CreateIntermediateLevel();
            CreateAdvancedLevel();
            CreateExpertLevel();
            CreateMasterLevel();

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Generated 5 random levels!", "OK");
        }


        private void CreateTutorialLevel()
        {
            LevelData level = CreateBaseLevelData("Level_01_Tutorial", "Tutorial Level", 1);
            level.ResizeGrid(12, 8);

            // Create simple straight path
            int pathY = 4;
            level.SetCellType(0, pathY, CellType.SpawnPoint);

            for (int x = 1; x < 11; x++)
            {
                level.SetCellType(x, pathY, CellType.Path);
            }

            level.SetCellType(11, pathY, CellType.ExitPoint);

            // Add basic obstacles
            level.SetCellType(3, 2, CellType.Obstacle);
            level.SetCellType(8, 6, CellType.Obstacle);

            // Get basic enemies (lowest health/damage)
            var basicEnemies = enemyData.OrderBy(e => e.Health + e.Damage).Take(1).ToList();
            AddWaves(level, 3, 1, 2, basicEnemies);

            SaveLevel(level);
        }

        private void CreateIntermediateLevel()
        {
            LevelData level = CreateBaseLevelData("Level_02_Intermediate", "L-Path Challenge", 2);
            level.ResizeGrid(15, 10);

            CreateLShapedPath(level);
            AddRandomObstacles(level, 4);

            // Get easy and medium difficulty enemies
            var intermediateEnemies = enemyData.OrderBy(e => e.Health + e.Damage).Take(2).ToList();
            AddWaves(level, 4, 2, 3, intermediateEnemies);

            SaveLevel(level);
        }

        private void CreateAdvancedLevel()
        {
            LevelData level = CreateBaseLevelData("Level_03_Advanced", "Winding Path", 3);
            level.ResizeGrid(16, 12);

            CreateWindingPath(level);
            AddRandomObstacles(level, 6);

            // Get medium difficulty enemies
            var advancedEnemies = enemyData.OrderBy(e => e.Health + e.Damage).Take(3).ToList();
            AddWaves(level, 5, 2, 4, advancedEnemies);

            SaveLevel(level);
        }

        private void CreateExpertLevel()
        {
            LevelData level = CreateBaseLevelData("Level_04_Expert", "Maze Runner", 4);
            level.ResizeGrid(18, 14);

            CreateMazePath(level);
            AddRandomObstacles(level, 8);

            // Use all enemy types except bosses
            var expertEnemies = enemyData.Where(e => !e.EnemyName.ToLower().Contains("boss")).ToList();
            AddWaves(level, 6, 3, 5, expertEnemies);

            SaveLevel(level);
        }

        private void CreateMasterLevel()
        {
            LevelData level = CreateBaseLevelData("Level_05_Master", "Dual Paths", 5);
            level.ResizeGrid(20, 15);

            CreateMultiPath(level);
            AddRandomObstacles(level, 10);

            // Find boss enemies
            var bossEnemies = enemyData.Where(e => e.EnemyName.ToLower().Contains("boss")).ToList();

            // Create master waves with boss encounters if boss enemies exist
            AddWaves(level, 7, 3, 6, enemyData, bossEnemies.Count > 0);

            SaveLevel(level);
        }

        private LevelData CreateBaseLevelData(string fileName, string levelName, int levelNumber)
        {
            LevelData level = ScriptableObject.CreateInstance<LevelData>();
            level.name = fileName;

            SerializedObject serializedObject = new SerializedObject(level);

            SetProperty(serializedObject, "levelName", levelName);
            SetProperty(serializedObject, "levelNumber", levelNumber);
            SetProperty(serializedObject, "startingCurrency", 300 + (levelNumber - 1) * 50);

            serializedObject.ApplyModifiedProperties();
            level.InitializeDefaultValues();

            return level;
        }

        private void CreateLShapedPath(LevelData level)
        {
            int turnPoint = level.GridWidth / 2;
            int pathY = level.GridHeight - 3;

            // Horizontal path
            level.SetCellType(0, pathY, CellType.SpawnPoint);
            for (int x = 1; x < turnPoint; x++)
            {
                level.SetCellType(x, pathY, CellType.Path);
            }

            // Vertical path
            for (int y = pathY - 1; y >= 2; y--)
            {
                level.SetCellType(turnPoint, y, CellType.Path);
            }

            // Final horizontal path
            for (int x = turnPoint + 1; x < level.GridWidth - 1; x++)
            {
                level.SetCellType(x, 2, CellType.Path);
            }

            level.SetCellType(level.GridWidth - 1, 2, CellType.ExitPoint);
        }

        private void CreateWindingPath(LevelData level)
        {
            List<Vector2Int> pathPoints = new List<Vector2Int>();
            int currentX = 0;
            int currentY = level.GridHeight / 2;

            // Create S-shaped path
            level.SetCellType(currentX, currentY, CellType.SpawnPoint);

            while (currentX < level.GridWidth - 1)
            {
                currentX++;
                if (currentX < level.GridWidth - 1)
                {
                    level.SetCellType(currentX, currentY, CellType.Path);
                }

                if (currentX % 5 == 0)
                {
                    // Change direction
                    int direction = (currentX / 5) % 2 == 0 ? 1 : -1;
                    for (int i = 0; i < 3; i++)
                    {
                        currentY += direction;
                        level.SetCellType(currentX, currentY, CellType.Path);
                    }
                }
            }

            level.SetCellType(currentX, currentY, CellType.ExitPoint);
        }

        private void CreateMazePath(LevelData level)
        {
            // Create a complex path with multiple turns
            int currentX = 0;
            int currentY = level.GridHeight / 2;

            level.SetCellType(currentX, currentY, CellType.SpawnPoint);

            while (currentX < level.GridWidth - 1)
            {
                // Move horizontally
                currentX++;
                if (currentX < level.GridWidth - 1)
                {
                    level.SetCellType(currentX, currentY, CellType.Path);
                }

                // Add random vertical movements
                if (Random.value > 0.7f)
                {
                    int verticalSteps = Random.Range(1, 4);
                    int direction = Random.value > 0.5f ? 1 : -1;

                    for (int i = 0; i < verticalSteps; i++)
                    {
                        int newY = currentY + (direction * i);
                        if (newY > 1 && newY < level.GridHeight - 2)
                        {
                            level.SetCellType(currentX, newY, CellType.Path);
                            currentY = newY;
                        }
                    }
                }
            }

            level.SetCellType(currentX, currentY, CellType.ExitPoint);
        }

        private void CreateMultiPath(LevelData level)
        {
            // Create two parallel paths
            int path1Y = level.GridHeight / 3;
            int path2Y = (level.GridHeight * 2) / 3;

            // First path
            level.SetCellType(0, path1Y, CellType.SpawnPoint);
            for (int x = 1; x < level.GridWidth - 1; x++)
            {
                level.SetCellType(x, path1Y, CellType.Path);
            }
            level.SetCellType(level.GridWidth - 1, path1Y, CellType.ExitPoint);

            // Second path
            level.SetCellType(0, path2Y, CellType.SpawnPoint);
            for (int x = 1; x < level.GridWidth - 1; x++)
            {
                level.SetCellType(x, path2Y, CellType.Path);
            }
            level.SetCellType(level.GridWidth - 1, path2Y, CellType.ExitPoint);

            // Add connecting paths
            int connection1 = level.GridWidth / 3;
            int connection2 = (level.GridWidth * 2) / 3;

            for (int y = path1Y + 1; y < path2Y; y++)
            {
                level.SetCellType(connection1, y, CellType.Path);
                level.SetCellType(connection2, y, CellType.Path);
            }
        }

        private void AddRandomObstacles(LevelData level, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x = Random.Range(0, level.GridWidth);
                int y = Random.Range(0, level.GridHeight);

                if (level.GetCellType(x, y) == CellType.Empty)
                {
                    level.SetCellType(x, y, CellType.Obstacle);
                }
            }
        }

        private void AddWaves(LevelData level, int waveCount, int minGroups, int maxGroups, List<EnemyData> availableEnemies, bool includeBoss = false)
        {
            if (availableEnemies == null || availableEnemies.Count == 0)
            {
                Debug.LogError("No enemy types available for wave generation!");
                return;
            }

            level.ClearWaves();

            for (int i = 0; i < waveCount; i++)
            {
                WaveData wave = new WaveData();
                int groupCount = Random.Range(minGroups, maxGroups + 1);

                for (int j = 0; j < groupCount; j++)
                {
                    EnemyGroupData group = new EnemyGroupData();

                    // Last wave, last group might be a boss
                    if (includeBoss && i == waveCount - 1 && j == groupCount - 1)
                    {
                        var bossEnemy = enemyData.FirstOrDefault(e => e.EnemyName.ToLower().Contains("boss")) ??
                                      availableEnemies[availableEnemies.Count - 1];
                        group.SetEnemyId(bossEnemy.EnemyId);
                        group.SetCount(1);
                    }
                    else
                    {
                        var selectedEnemy = availableEnemies[Random.Range(0, availableEnemies.Count)];
                        group.SetEnemyId(selectedEnemy.EnemyId);
                        group.SetCount(Random.Range(3, 8));
                    }

                    group.SetSpawnDelay(Random.Range(1f, 2f));
                    wave.AddEnemyGroup(group);
                }

                wave.SetDelayBetweenGroups(Random.Range(2f, 5f));
                level.AddWave(wave);
            }
        }

        private void SetProperty(SerializedObject serializedObject, string propertyName, object value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                switch (value)
                {
                    case int intValue:
                        property.intValue = intValue;
                        break;
                    case string stringValue:
                        property.stringValue = stringValue;
                        break;
                    case float floatValue:
                        property.floatValue = floatValue;
                        break;
                }
            }
        }

        private void SaveLevel(LevelData level)
        {
            string assetPath = $"{savePath}{level.name}.asset";
            AssetDatabase.CreateAsset(level, assetPath);

            string jsonPath = $"{savePath}{level.name}.json";
            File.WriteAllText(jsonPath, level.ToJson());

            Debug.Log($"Created level: {level.name}");
        }
    }
}