#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using TowerDefense.Grid;
using TowerDefense.Level;

namespace TowerDefense.Editor
{
    /// <summary>
    /// Editor tool for creating and editing levels
    /// </summary>
    public class LevelEditor : EditorWindow
    {
        private LevelData _currentLevel;
        private int _selectedCellTypeIndex = 0;
        private Vector2 _scrollPosition;
        private bool _showGridEditor = true;
        private bool _showWaveEditor = true;
        private CellType[] _cellTypes = { CellType.Empty, CellType.Path, CellType.SpawnPoint, CellType.ExitPoint, CellType.Obstacle };
        private string[] _cellTypeNames = { "Empty", "Path", "Spawn Point", "Exit Point", "Obstacle" };

        [MenuItem("Tools/Tower Defense/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditor>("Level Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Tower Defense Level Editor", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            // Level selection/creation
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("New Level", GUILayout.Width(100)))
            {
                CreateNewLevel();
            }

            if (GUILayout.Button("Load Level", GUILayout.Width(100)))
            {
                LoadLevelFromFile();
            }

            if (GUILayout.Button("Save Level", GUILayout.Width(100)))
            {
                SaveLevelToFile();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (_currentLevel == null)
            {
                EditorGUILayout.HelpBox("Create a new level or load an existing one.", MessageType.Info);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Level properties
            EditorGUILayout.LabelField("Level Properties", EditorStyles.boldLabel);
            _currentLevel.name = EditorGUILayout.TextField("Level Name", _currentLevel.name);

            SerializedObject serializedObject = new SerializedObject(_currentLevel);
            SerializedProperty levelNumberProperty = serializedObject.FindProperty("_levelNumber");
            SerializedProperty startingCurrencyProperty = serializedObject.FindProperty("_startingCurrency");

            EditorGUILayout.PropertyField(levelNumberProperty, new GUIContent("Level Number"));
            EditorGUILayout.PropertyField(startingCurrencyProperty, new GUIContent("Starting Currency"));

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            // Grid Editor
            _showGridEditor = EditorGUILayout.Foldout(_showGridEditor, "Grid Editor");
            if (_showGridEditor)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Cell Type", GUILayout.Width(80));
                _selectedCellTypeIndex = EditorGUILayout.Popup(_selectedCellTypeIndex, _cellTypeNames);

                EditorGUILayout.EndHorizontal();

                // Grid view
                EditorGUILayout.LabelField("Grid Layout (Click to set cell type)");
                DrawGridEditor();
            }

            EditorGUILayout.Space();

            // Wave Editor
            _showWaveEditor = EditorGUILayout.Foldout(_showWaveEditor, "Wave Editor");
            if (_showWaveEditor)
            {
                DrawWaveEditor();
            }

            EditorGUILayout.EndScrollView();
        }

        private void CreateNewLevel()
        {
            _currentLevel = ScriptableObject.CreateInstance<LevelData>();
            _currentLevel.InitializeDefaultValues();
        }

        private void LoadLevelFromFile()
        {
            string path = EditorUtility.OpenFilePanel("Load Level", Application.dataPath, "json");
            if (string.IsNullOrEmpty(path))
                return;

            string json = File.ReadAllText(path);
            _currentLevel = LevelData.FromJson(json);
        }

        private void SaveLevelToFile()
        {
            if (_currentLevel == null)
                return;

            string path = EditorUtility.SaveFilePanel("Save Level", Application.dataPath, _currentLevel.name + ".json", "json");
            if (string.IsNullOrEmpty(path))
                return;

            string json = _currentLevel.ToJson();
            File.WriteAllText(path, json);

            // Also save as asset
            string assetPath = "Assets/Resources/Levels/" + _currentLevel.name + ".asset";

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(assetPath));

            AssetDatabase.CreateAsset(_currentLevel, assetPath);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Level Saved", "Level saved to:\n" + path + "\n\nAnd as asset:\n" + assetPath, "OK");
        }

        private void DrawGridEditor()
        {
            if (_currentLevel == null)
                return;

            int gridWidth = _currentLevel.GridWidth;
            int gridHeight = _currentLevel.GridHeight;

            // Calculate cell size based on window width
            float cellSize = Mathf.Min(20, (position.width - 40) / gridWidth);

            // Grid background
            Rect gridRect = EditorGUILayout.GetControlRect(GUILayout.Height(cellSize * gridHeight + 1));
            Rect cellRect = new Rect(gridRect.x, gridRect.y, cellSize, cellSize);

            EditorGUI.DrawRect(new Rect(gridRect.x - 1, gridRect.y - 1, gridRect.width + 2, gridRect.height + 2), Color.black);

            // Draw grid cells
            for (int z = 0; z < gridHeight; z++)
            {
                cellRect.x = gridRect.x;

                for (int x = 0; x < gridWidth; x++)
                {
                    CellType cellType = _currentLevel.GetCellType(x, z);
                    Color cellColor = GetCellColor(cellType);

                    EditorGUI.DrawRect(cellRect, cellColor);

                    // Handle clicks
                    if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                    {
                        _currentLevel.SetCellType(x, z, _cellTypes[_selectedCellTypeIndex]);
                        GUI.changed = true;
                        Repaint();
                    }

                    cellRect.x += cellSize;
                }

                cellRect.y += cellSize;
            }
        }

        private void DrawWaveEditor()
        {
            if (_currentLevel == null)
                return;

            SerializedObject serializedObject = new SerializedObject(_currentLevel);
            SerializedProperty wavesProperty = serializedObject.FindProperty("_waves");

            EditorGUILayout.PropertyField(wavesProperty, new GUIContent("Waves"), true);

            if (GUILayout.Button("Add Wave"))
            {
                WaveData newWave = new WaveData();
                newWave.AddEnemyGroup(new EnemyGroupData());
                _currentLevel.AddWave(newWave);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private Color GetCellColor(CellType cellType)
        {
            switch (cellType)
            {
                case CellType.Empty:
                    return new Color(0.8f, 0.8f, 0.8f);
                case CellType.Path:
                    return new Color(0.6f, 0.4f, 0.2f);
                case CellType.SpawnPoint:
                    return Color.green;
                case CellType.ExitPoint:
                    return Color.red;
                case CellType.Obstacle:
                    return new Color(0.3f, 0.3f, 0.3f);
                default:
                    return Color.white;
            }
        }
    }
}
#endif