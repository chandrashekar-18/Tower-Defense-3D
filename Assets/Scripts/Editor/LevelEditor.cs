#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using TowerDefense.Grid;
using TowerDefense.Level;

namespace TowerDefense.Editor
{
    /// <summary>
    /// Editor tool for creating and editing Tower Defense levels.
    /// </summary>
    public class LevelEditor : EditorWindow
    {
        #region Private Fields
        private LevelData currentLevel;
        private int selectedCellTypeIndex = 0;
        private Vector2 scrollPosition;
        private bool showGridEditor = true;
        private bool showWaveEditor = true;

        private readonly CellType[] cellTypes =
        {
            CellType.Empty,
            CellType.Path,
            CellType.SpawnPoint,
            CellType.ExitPoint,
            CellType.Obstacle
        };

        private readonly string[] cellTypeNames =
        {
            "Empty",
            "Path",
            "Spawn Point",
            "Exit Point",
            "Obstacle"
        };
        #endregion

        #region Menu
        [MenuItem("Tools/Tower Defense/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditor>("Level Editor");
        }
        #endregion

        #region Unity Callbacks
        private void OnGUI()
        {
            GUILayout.Label("Tower Defense Level Editor", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            DrawLevelButtons();

            if (currentLevel == null)
            {
                EditorGUILayout.HelpBox("Create a new level or load an existing one.", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawLevelProperties();
            DrawGridEditorSection();
            DrawWaveEditorSection();

            EditorGUILayout.EndScrollView();
        }
        #endregion

        #region Level Buttons
        private void DrawLevelButtons()
        {
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
        }
        #endregion

        #region Level Operations
        private void CreateNewLevel()
        {
            currentLevel = ScriptableObject.CreateInstance<LevelData>();
            currentLevel.InitializeDefaultValues();
        }

        private void LoadLevelFromFile()
        {
            string path = EditorUtility.OpenFilePanel("Load Level", Application.dataPath, "json");
            if (string.IsNullOrEmpty(path))
                return;

            string json = File.ReadAllText(path);
            currentLevel = LevelData.FromJson(json);
        }

        private void SaveLevelToFile()
        {
            if (currentLevel == null)
                return;

            string path = EditorUtility.SaveFilePanel(
                "Save Level",
                Application.dataPath,
                currentLevel.name + ".json",
                "json"
            );

            if (string.IsNullOrEmpty(path))
                return;

            string json = currentLevel.ToJson();
            File.WriteAllText(path, json);

            // Also save as asset
            string assetPath = "Assets/Resources/Levels/" + currentLevel.name + ".asset";

            Directory.CreateDirectory(Path.GetDirectoryName(assetPath));

            AssetDatabase.CreateAsset(currentLevel, assetPath);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog(
                "Level Saved",
                $"Level saved to:\n{path}\n\nAnd as asset:\n{assetPath}",
                "OK"
            );
        }
        #endregion

        #region Level Properties
        private void DrawLevelProperties()
        {
            EditorGUILayout.LabelField("Level Properties", EditorStyles.boldLabel);

            currentLevel.name = EditorGUILayout.TextField("Level Name", currentLevel.name);

            SerializedObject serializedObject = new SerializedObject(currentLevel);
            SerializedProperty levelNumberProperty = serializedObject.FindProperty("levelNumber");
            SerializedProperty startingCurrencyProperty = serializedObject.FindProperty("startingCurrency");

            EditorGUILayout.PropertyField(levelNumberProperty, new GUIContent("Level Number"));
            EditorGUILayout.PropertyField(startingCurrencyProperty, new GUIContent("Starting Currency"));

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
        }
        #endregion

        #region Grid Editor
        private void DrawGridEditorSection()
        {
            showGridEditor = EditorGUILayout.Foldout(showGridEditor, "Grid Editor");
            if (!showGridEditor || currentLevel == null)
                return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Cell Type", GUILayout.Width(80));
            selectedCellTypeIndex = EditorGUILayout.Popup(selectedCellTypeIndex, cellTypeNames);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Grid Layout (Click to set cell type)");
            DrawGridEditor();
        }

        private void DrawGridEditor()
        {
            int gridWidth = currentLevel.GridWidth;
            int gridHeight = currentLevel.GridHeight;

            float cellSize = Mathf.Min(20, (position.width - 40) / gridWidth);
            Rect gridRect = EditorGUILayout.GetControlRect(GUILayout.Height(cellSize * gridHeight + 1));
            Rect cellRect = new Rect(gridRect.x, gridRect.y, cellSize, cellSize);

            EditorGUI.DrawRect(new Rect(gridRect.x - 1, gridRect.y - 1, gridRect.width + 2, gridRect.height + 2), Color.black);

            for (int z = 0; z < gridHeight; z++)
            {
                cellRect.x = gridRect.x;

                for (int x = 0; x < gridWidth; x++)
                {
                    CellType cellType = currentLevel.GetCellType(x, z);
                    EditorGUI.DrawRect(cellRect, GetCellColor(cellType));

                    if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                    {
                        currentLevel.SetCellType(x, z, cellTypes[selectedCellTypeIndex]);
                        GUI.changed = true;
                        Repaint();
                    }

                    cellRect.x += cellSize;
                }

                cellRect.y += cellSize;
            }
        }

        private Color GetCellColor(CellType cellType)
        {
            return cellType switch
            {
                CellType.Empty => new Color(0.8f, 0.8f, 0.8f),
                CellType.Path => new Color(0.6f, 0.4f, 0.2f),
                CellType.SpawnPoint => Color.green,
                CellType.ExitPoint => Color.red,
                CellType.Obstacle => new Color(0.3f, 0.3f, 0.3f),
                _ => Color.white
            };
        }
        #endregion

        #region Wave Editor
        private void DrawWaveEditorSection()
        {
            showWaveEditor = EditorGUILayout.Foldout(showWaveEditor, "Wave Editor");
            if (!showWaveEditor || currentLevel == null)
                return;

            DrawWaveEditor();
        }

        private void DrawWaveEditor()
        {
            SerializedObject serializedObject = new SerializedObject(currentLevel);
            SerializedProperty wavesProperty = serializedObject.FindProperty("waves");

            EditorGUILayout.PropertyField(wavesProperty, new GUIContent("Waves"), true);

            if (GUILayout.Button("Add Wave"))
            {
                WaveData newWave = new WaveData();
                newWave.AddEnemyGroup(new EnemyGroupData());
                currentLevel.AddWave(newWave);
            }

            serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}
#endif
