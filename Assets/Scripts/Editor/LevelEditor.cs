#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using TowerDefense.Grid;
using TowerDefense.Level;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefense.Editor
{
    /// <summary>
    /// Enhanced editor tool for creating and editing Tower Defense levels.
    /// </summary>
    public class LevelEditor : EditorWindow
    {
        #region Private Fields
        private LevelData currentLevel;
        private int selectedCellTypeIndex = 0;
        private Vector2 scrollPosition;
        private bool showLevelProperties = true;
        private bool showGridEditor = true;
        private bool showWaveEditor = true;
        private bool showPathSettings = false;

        // Path generation settings
        private int pathComplexity = 3;
        private bool allowDiagonals = false;
        private int minPathLength = 10;

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

        // Custom styles
        private GUIStyle headerStyle;
        private GUIStyle buttonStyle;
        private GUIStyle foldoutStyle;
        private GUIStyle boxStyle;
        private bool stylesInitialized = false;
        #endregion

        #region Menu
        [MenuItem("Tools/Tower Defense/Level Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<LevelEditor>("Level Editor");
            window.minSize = new Vector2(400, 600);
        }
        #endregion

        #region Unity Callbacks
        private void OnGUI()
        {
            InitializeStyles();

            // Header
            EditorGUILayout.BeginVertical(boxStyle);
            GUILayout.Label("🏰 Tower Defense Level Editor", headerStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            DrawLevelButtons();

            if (currentLevel == null)
            {
                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUILayout.HelpBox("🎮 Ready to create an epic level?\nCreate a new level or load an existing one to get started!", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawLevelProperties();
            DrawGridEditorSection();
            DrawWaveEditorSection();

            EditorGUILayout.EndScrollView();
        }
        #endregion

        #region Styles
        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.2f, 0.6f, 1f) }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30
            };

            foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.3f, 0.7f, 0.9f) }
            };

            boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(5, 5, 5, 5)
            };

            stylesInitialized = true;
        }
        #endregion

        #region Level Buttons
        private void DrawLevelButtons()
        {
            EditorGUILayout.BeginVertical(boxStyle);

            EditorGUILayout.BeginHorizontal();

            // New Level Button
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("📄 New Level", buttonStyle, GUILayout.Width(120)))
            {
                CreateNewLevel();
            }

            // Load Level Button
            GUI.backgroundColor = new Color(0.4f, 0.6f, 0.8f);
            if (GUILayout.Button("📁 Load Level", buttonStyle, GUILayout.Width(120)))
            {
                LoadLevelFromFile();
            }

            // Save Level Button
            GUI.backgroundColor = new Color(0.8f, 0.6f, 0.4f);
            if (GUILayout.Button("💾 Save Level", buttonStyle, GUILayout.Width(120)))
            {
                SaveLevelToFile();
            }

            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
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
                "Level Saved Successfully! 🎉",
                $"Level saved to:\n{path}\n\nAnd as asset:\n{assetPath}",
                "Awesome!"
            );
        }
        #endregion

        #region Level Properties
        private void DrawLevelProperties()
        {
            EditorGUILayout.BeginVertical(boxStyle);

            showLevelProperties = EditorGUILayout.Foldout(showLevelProperties, "📊 Level Properties", foldoutStyle);

            if (showLevelProperties)
            {
                EditorGUILayout.Space(5);

                currentLevel.name = EditorGUILayout.TextField("🏷️ Level Name", currentLevel.name);

                SerializedObject serializedObject = new SerializedObject(currentLevel);
                SerializedProperty levelNumberProperty = serializedObject.FindProperty("levelNumber");
                SerializedProperty startingCurrencyProperty = serializedObject.FindProperty("startingCurrency");
                SerializedProperty gridWidthProperty = serializedObject.FindProperty("gridWidth");
                SerializedProperty gridHeightProperty = serializedObject.FindProperty("gridHeight");

                EditorGUILayout.PropertyField(levelNumberProperty, new GUIContent("🔢 Level Number"));
                EditorGUILayout.PropertyField(startingCurrencyProperty, new GUIContent("💰 Starting Currency"));

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("📐 Grid Size", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();

                // Grid size sliders with value labels
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Width", GUILayout.Width(50));
                int newWidth = EditorGUILayout.IntSlider(gridWidthProperty.intValue, 5, 30);
                EditorGUILayout.LabelField(newWidth.ToString(), GUILayout.Width(30));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Height", GUILayout.Width(50));
                int newHeight = EditorGUILayout.IntSlider(gridHeightProperty.intValue, 5, 30);
                EditorGUILayout.LabelField(newHeight.ToString(), GUILayout.Width(30));
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    if (newWidth != gridWidthProperty.intValue || newHeight != gridHeightProperty.intValue)
                    {
                        Undo.RecordObject(currentLevel, "Resize Grid");
                        currentLevel.ResizeGrid(newWidth, newHeight);
                        EditorUtility.SetDirty(currentLevel);
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        #endregion

        #region Grid Editor
        private void DrawGridEditorSection()
        {
            EditorGUILayout.BeginVertical(boxStyle);

            showGridEditor = EditorGUILayout.Foldout(showGridEditor, "🗺️ Grid Editor", foldoutStyle);
            if (!showGridEditor || currentLevel == null)
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
                return;
            }

            EditorGUILayout.Space(5);

            // Cell type selector with icons
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("🎨 Paint Tool", GUILayout.Width(80));

            string[] cellTypeDisplayNames = {
                "⬜ Empty",
                "🛤️ Path",
                "🟢 Spawn",
                "🔴 Exit",
                "⬛ Obstacle"
            };

            selectedCellTypeIndex = EditorGUILayout.Popup(selectedCellTypeIndex, cellTypeDisplayNames);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Grid manipulation buttons
            DrawGridManipulationButtons();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("🖱️ Grid Layout (Click to paint cells)", EditorStyles.boldLabel);
            EditorGUILayout.Space(20); // Added extra space for grid coordinates
            DrawGridEditor();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawGridManipulationButtons()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("⚡ Quick Actions", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            // Clear Grid
            GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
            if (GUILayout.Button("🧹 Clear Grid", buttonStyle))
            {
                if (EditorUtility.DisplayDialog("Clear Grid", "Are you sure you want to clear the entire grid?", "Yes", "No"))
                {
                    ClearGrid();
                }
            }

            // Generate Random Path
            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
            if (GUILayout.Button("🎲 Random Path", buttonStyle))
            {
                GenerateRandomPath();
            }

            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            // Path generation settings
            showPathSettings = EditorGUILayout.Foldout(showPathSettings, "⚙️ Path Generation Settings");
            if (showPathSettings)
            {
                EditorGUI.indentLevel++;
                pathComplexity = EditorGUILayout.IntSlider("🌀 Path Complexity", pathComplexity, 1, 5);
                allowDiagonals = EditorGUILayout.Toggle("↗️ Allow Diagonals", allowDiagonals);
                minPathLength = EditorGUILayout.IntSlider("📏 Min Path Length", minPathLength, 5, 20);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawGridEditor()
        {
            int gridWidth = currentLevel.GridWidth;
            int gridHeight = currentLevel.GridHeight;

            float cellSize = Mathf.Min(25, (position.width - 60) / gridWidth);
            Rect gridRect = EditorGUILayout.GetControlRect(GUILayout.Height(cellSize * gridHeight + 4));

            // Draw grid background
            EditorGUI.DrawRect(new Rect(gridRect.x - 2, gridRect.y - 2, gridRect.width + 4, gridRect.height + 4), new Color(0.2f, 0.2f, 0.2f));

            Rect cellRect = new Rect(gridRect.x, gridRect.y, cellSize, cellSize);

            for (int z = 0; z < gridHeight; z++)
            {
                cellRect.x = gridRect.x;

                for (int x = 0; x < gridWidth; x++)
                {
                    CellType cellType = currentLevel.GetCellType(x, z);
                    Color cellColor = GetCellColor(cellType);

                    // Draw cell background
                    EditorGUI.DrawRect(cellRect, cellColor);

                    // Draw cell border
                    EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, cellRect.width, 1), Color.black);
                    EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, 1, cellRect.height), Color.black);
                    EditorGUI.DrawRect(new Rect(cellRect.x + cellRect.width - 1, cellRect.y, 1, cellRect.height), Color.black);
                    EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y + cellRect.height - 1, cellRect.width, 1), Color.black);

                    // Draw cell icon
                    string icon = GetCellIcon(cellType);
                    if (!string.IsNullOrEmpty(icon))
                    {
                        GUI.Label(cellRect, icon, new GUIStyle { alignment = TextAnchor.MiddleCenter, fontSize = Mathf.RoundToInt(cellSize * 0.6f) });
                    }

                    // Handle mouse clicks
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

            // Draw grid coordinates
            DrawGridCoordinates(gridRect, cellSize, gridWidth, gridHeight);
        }

        private void DrawGridCoordinates(Rect gridRect, float cellSize, int gridWidth, int gridHeight)
        {
            GUIStyle coordStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 8
            };

            // Draw X coordinates (top)
            for (int x = 0; x < gridWidth; x++)
            {
                Rect coordRect = new Rect(gridRect.x + x * cellSize, gridRect.y - 15, cellSize, 12);
                GUI.Label(coordRect, x.ToString(), coordStyle);
            }

            // Draw Z coordinates (left)
            for (int z = 0; z < gridHeight; z++)
            {
                Rect coordRect = new Rect(gridRect.x - 20, gridRect.y + z * cellSize, 18, cellSize);
                GUI.Label(coordRect, z.ToString(), coordStyle);
            }
        }

        private Color GetCellColor(CellType cellType)
        {
            return cellType switch
            {
                CellType.Empty => new Color(0.9f, 0.9f, 0.9f),
                CellType.Path => new Color(0.8f, 0.6f, 0.4f),
                CellType.SpawnPoint => new Color(0.4f, 0.8f, 0.4f),
                CellType.ExitPoint => new Color(0.8f, 0.4f, 0.4f),
                CellType.Obstacle => new Color(0.4f, 0.4f, 0.4f),
                _ => Color.white
            };
        }

        private string GetCellIcon(CellType cellType)
        {
            return cellType switch
            {
                CellType.Empty => "",
                CellType.Path => "→",
                CellType.SpawnPoint => "🚪",
                CellType.ExitPoint => "🏁",
                CellType.Obstacle => "🧱",
                _ => ""
            };
        }
        #endregion

        #region Grid Manipulation
        private void ClearGrid()
        {
            for (int x = 0; x < currentLevel.GridWidth; x++)
            {
                for (int z = 0; z < currentLevel.GridHeight; z++)
                {
                    currentLevel.SetCellType(x, z, CellType.Empty);
                }
            }
            EditorUtility.SetDirty(currentLevel);
        }

        private void GenerateRandomPath()
        {
            if (currentLevel == null) return;

            // Clear existing path elements
            ClearPathElements();

            // Generate path using A* inspired algorithm with randomness
            List<Vector2Int> pathPoints = GenerateRandomPathPoints();

            if (pathPoints.Count < 2)
            {
                EditorUtility.DisplayDialog("Path Generation Failed", "Could not generate a valid path. Try adjusting the settings.", "OK");
                return;
            }

            // Set spawn point
            Vector2Int spawnPoint = pathPoints[0];
            currentLevel.SetCellType(spawnPoint.x, spawnPoint.y, CellType.SpawnPoint);

            // Set exit point
            Vector2Int exitPoint = pathPoints[pathPoints.Count - 1];
            currentLevel.SetCellType(exitPoint.x, exitPoint.y, CellType.ExitPoint);

            // Set path points
            for (int i = 1; i < pathPoints.Count - 1; i++)
            {
                Vector2Int point = pathPoints[i];
                currentLevel.SetCellType(point.x, point.y, CellType.Path);
            }

            // Add some random obstacles
            AddRandomObstacles();

            EditorUtility.SetDirty(currentLevel);
            EditorUtility.DisplayDialog("Path Generated! 🎉", $"Successfully generated a path with {pathPoints.Count} points!", "Awesome!");
        }

        private void ClearPathElements()
        {
            for (int x = 0; x < currentLevel.GridWidth; x++)
            {
                for (int z = 0; z < currentLevel.GridHeight; z++)
                {
                    CellType cellType = currentLevel.GetCellType(x, z);
                    if (cellType == CellType.Path || cellType == CellType.SpawnPoint || cellType == CellType.ExitPoint)
                    {
                        currentLevel.SetCellType(x, z, CellType.Empty);
                    }
                }
            }
        }

        private List<Vector2Int> GenerateRandomPathPoints()
        {
            List<Vector2Int> pathPoints = new List<Vector2Int>();

            // Choose random spawn point (left edge)
            Vector2Int current = new Vector2Int(0, Random.Range(1, currentLevel.GridHeight - 1));
            pathPoints.Add(current);

            // Generate path segments
            for (int segment = 0; segment < pathComplexity; segment++)
            {
                Vector2Int target = GenerateSegmentTarget(current, segment);
                List<Vector2Int> segmentPath = GeneratePathSegment(current, target);

                // Add segment points (skip first to avoid duplicates)
                for (int i = 1; i < segmentPath.Count; i++)
                {
                    pathPoints.Add(segmentPath[i]);
                }

                current = target;
            }

            // Ensure path reaches the right edge
            Vector2Int finalTarget = new Vector2Int(currentLevel.GridWidth - 1, current.y);
            List<Vector2Int> finalSegment = GeneratePathSegment(current, finalTarget);

            for (int i = 1; i < finalSegment.Count; i++)
            {
                pathPoints.Add(finalSegment[i]);
            }

            return pathPoints;
        }

        private Vector2Int GenerateSegmentTarget(Vector2Int current, int segmentIndex)
        {
            int gridWidth = currentLevel.GridWidth;
            int gridHeight = currentLevel.GridHeight;

            // Progress towards the right edge
            float progressRatio = (float)(segmentIndex + 1) / (pathComplexity + 1);
            int targetX = Mathf.RoundToInt(Mathf.Lerp(current.x, gridWidth - 1, progressRatio));
            targetX = Mathf.Clamp(targetX, current.x + 1, gridWidth - 1);

            // Add some vertical variation
            int verticalVariation = Random.Range(-2, 3);
            int targetZ = Mathf.Clamp(current.y + verticalVariation, 1, gridHeight - 2);

            return new Vector2Int(targetX, targetZ);
        }

        private List<Vector2Int> GeneratePathSegment(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> segment = new List<Vector2Int>();
            Vector2Int current = start;
            segment.Add(current);

            while (current != end)
            {
                // Choose next step
                Vector2Int direction = GetNextDirection(current, end);
                current += direction;

                // Ensure we stay within bounds
                current.x = Mathf.Clamp(current.x, 0, currentLevel.GridWidth - 1);
                current.y = Mathf.Clamp(current.y, 0, currentLevel.GridHeight - 1);

                segment.Add(current);

                // Safety check to prevent infinite loops
                if (segment.Count > currentLevel.GridWidth + currentLevel.GridHeight)
                    break;
            }

            return segment;
        }

        private Vector2Int GetNextDirection(Vector2Int current, Vector2Int target)
        {
            List<Vector2Int> possibleDirections = new List<Vector2Int>();

            // Basic directions
            Vector2Int[] directions = {
                Vector2Int.right,
                Vector2Int.left,
                Vector2Int.up,
                Vector2Int.down
            };

            if (allowDiagonals)
            {
                directions = directions.Concat(new Vector2Int[] {
                    new Vector2Int(1, 1),
                    new Vector2Int(1, -1),
                    new Vector2Int(-1, 1),
                    new Vector2Int(-1, -1)
                }).ToArray();
            }

            // Add directions that move towards target
            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (IsValidGridPosition(next))
                {
                    float currentDistance = Vector2Int.Distance(current, target);
                    float nextDistance = Vector2Int.Distance(next, target);

                    if (nextDistance < currentDistance)
                    {
                        possibleDirections.Add(dir);
                    }
                }
            }

            // If no good directions, add any valid direction
            if (possibleDirections.Count == 0)
            {
                foreach (Vector2Int dir in directions)
                {
                    Vector2Int next = current + dir;
                    if (IsValidGridPosition(next))
                    {
                        possibleDirections.Add(dir);
                    }
                }
            }

            // Return random direction from possibilities
            if (possibleDirections.Count > 0)
            {
                return possibleDirections[Random.Range(0, possibleDirections.Count)];
            }

            return Vector2Int.zero;
        }

        private bool IsValidGridPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < currentLevel.GridWidth &&
                   pos.y >= 0 && pos.y < currentLevel.GridHeight;
        }

        private void AddRandomObstacles()
        {
            int obstacleCount = Random.Range(3, 8);

            for (int i = 0; i < obstacleCount; i++)
            {
                Vector2Int randomPos = new Vector2Int(
                    Random.Range(0, currentLevel.GridWidth),
                    Random.Range(0, currentLevel.GridHeight)
                );

                // Only place obstacles on empty cells
                if (currentLevel.GetCellType(randomPos.x, randomPos.y) == CellType.Empty)
                {
                    currentLevel.SetCellType(randomPos.x, randomPos.y, CellType.Obstacle);
                }
            }
        }
        #endregion

        #region Wave Editor
        private void DrawWaveEditorSection()
        {
            EditorGUILayout.BeginVertical(boxStyle);

            showWaveEditor = EditorGUILayout.Foldout(showWaveEditor, "🌊 Wave Editor", foldoutStyle);
            if (!showWaveEditor || currentLevel == null)
            {
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.Space(5);
            DrawWaveEditor();

            EditorGUILayout.EndVertical();
        }

        private void DrawWaveEditor()
        {
            SerializedObject serializedObject = new SerializedObject(currentLevel);
            SerializedProperty wavesProperty = serializedObject.FindProperty("waves");

            EditorGUILayout.PropertyField(wavesProperty, new GUIContent("🌊 Enemy Waves"), true);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = new Color(0.6f, 0.8f, 1f);
            if (GUILayout.Button("➕ Add Wave", buttonStyle, GUILayout.Width(120)))
            {
                WaveData newWave = new WaveData();
                newWave.AddEnemyGroup(new EnemyGroupData());
                currentLevel.AddWave(newWave);
            }

            GUI.backgroundColor = new Color(1f, 0.8f, 0.6f);
            if (GUILayout.Button("🎲 Random Waves", buttonStyle, GUILayout.Width(120)))
            {
                GenerateRandomWaves();
            }

            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            // Display wave summary
            if (currentLevel.Waves.Count > 0)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("📈 Wave Summary", EditorStyles.boldLabel);

                for (int i = 0; i < currentLevel.Waves.Count; i++)
                {
                    WaveData wave = currentLevel.Waves[i];
                    int totalEnemies = wave.GetTotalEnemyCount();
                    EditorGUILayout.LabelField($"Wave {i + 1}: {totalEnemies} enemies");
                }

                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void GenerateRandomWaves()
        {
            if (!EditorUtility.DisplayDialog("Generate Random Waves", "This will replace all existing waves. Continue?", "Yes", "No"))
                return;

            currentLevel.ClearWaves();

            int waveCount = Random.Range(3, 8);
            string[] enemyTypes = { "BasicEnemy", "FastEnemy", "TankEnemy", "FlyingEnemy" };

            for (int i = 0; i < waveCount; i++)
            {
                WaveData newWave = new WaveData();

                int groupCount = Random.Range(1, 4);
                for (int j = 0; j < groupCount; j++)
                {
                    EnemyGroupData group = new EnemyGroupData();
                    // Note: You'll need to modify EnemyGroupData to have public setters or constructor parameters
                    // This is a placeholder showing the intended functionality
                    newWave.AddEnemyGroup(group);
                }

                currentLevel.AddWave(newWave);
            }

            EditorUtility.DisplayDialog("Waves Generated! 🌊", $"Generated {waveCount} random waves!", "Great!");
        }
        #endregion
    }
}
#endif