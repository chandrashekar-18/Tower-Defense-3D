using UnityEngine;
using TowerDefense.Level;
using System.Collections.Generic;

namespace TowerDefense.Grid
{
    /// <summary>
    /// Manages the game grid and pathfinding.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        #region Singleton
        public static GridManager Instance { get; private set; }
        #endregion

        #region Variables
        [SerializeField] private GameObject gridCellPrefab;
        [SerializeField] private GameObject pathCellPrefab;
        [SerializeField] private GameObject spawnPointPrefab;
        [SerializeField] private GameObject exitPointPrefab;
        [SerializeField] private GameObject obstaclePrefab;
        [SerializeField] private float cellSize = 1f;
        private Transform gridContainer;
        private int gridWidth = 15;
        private int gridHeight = 10;
        private GridCell[,] grid;
        private List<Transform> spawnPoints = new List<Transform>();
        private List<Transform> exitPoints = new List<Transform>();
        private Dictionary<Transform, List<Vector3>> pathsFromSpawns = new Dictionary<Transform, List<Vector3>>();
        #endregion

        #region Properties
        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public float CellSize => cellSize;
        #endregion

        #region Events
        public delegate void GridGeneratedDelegate(GridCell[,] grid);
        public event GridGeneratedDelegate OnGridGenerated;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        private void Start()
        {
            GenerateGrid(LevelManager.Instance.CurrentLevelData);
        }
        #endregion

        #region Public Methods
        public void GenerateGrid(LevelData levelData)
        {
            ClearGrid();

            gridWidth = levelData.GridWidth;
            gridHeight = levelData.GridHeight;

            if (gridContainer == null)
            {
                GameObject container = new GameObject("GridContainer");
                gridContainer = container.transform;
                gridContainer.position = Vector3.zero;
            }

            grid = new GridCell[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);

                    CellType cellType = levelData.GetCellType(x, z);

                    GameObject cellPrefab = gridCellPrefab;

                    switch (cellType)
                    {
                        case CellType.Path:
                            cellPrefab = pathCellPrefab;
                            break;
                        case CellType.SpawnPoint:
                            cellPrefab = spawnPointPrefab;
                            break;
                        case CellType.ExitPoint:
                            cellPrefab = exitPointPrefab;
                            break;
                        case CellType.Obstacle:
                            cellPrefab = obstaclePrefab;
                            break;
                    }

                    GameObject cellObject = Instantiate(cellPrefab, position, Quaternion.identity, gridContainer);
                    cellObject.name = $"Cell_{x}_{z}";

                    GridCell cell = cellObject.GetComponent<GridCell>();
                    if (cell == null)
                    {
                        cell = cellObject.AddComponent<GridCell>();
                    }

                    cell.Initialize(x, z, cellType, position);
                    grid[x, z] = cell;

                    if (cellType == CellType.SpawnPoint)
                    {
                        spawnPoints.Add(cellObject.transform);
                    }
                    else if (cellType == CellType.ExitPoint)
                    {
                        exitPoints.Add(cellObject.transform);
                    }
                }
            }

            CalculatePathsFromSpawnPoints();

            OnGridGenerated?.Invoke(grid);
        }

        public bool CanPlaceTower(int x, int z)
        {
            if (x < 0 || x >= gridWidth || z < 0 || z >= gridHeight)
                return false;

            return grid[x, z].CellType == CellType.Empty && !grid[x, z].HasTower;
        }

        public bool CanPlaceTower(Vector3 worldPosition)
        {
            if (GetGridCoordinates(worldPosition, out int x, out int z))
            {
                return CanPlaceTower(x, z);
            }

            return false;
        }

        public bool PlaceTower(int x, int z, GameObject tower)
        {
            if (!CanPlaceTower(x, z))
                return false;

            Vector3 position = grid[x, z].transform.position;
            position.y += cellSize / 2;

            tower.transform.position = position;
            grid[x, z].SetTower(tower);

            return true;
        }

        public bool PlaceTower(Vector3 worldPosition, GameObject tower)
        {
            if (GetGridCoordinates(worldPosition, out int x, out int z))
            {
                return PlaceTower(x, z, tower);
            }

            return false;
        }

        public bool GetGridCoordinates(Vector3 worldPosition, out int x, out int z)
        {
            x = Mathf.FloorToInt(worldPosition.x / cellSize);
            z = Mathf.FloorToInt(worldPosition.z / cellSize);

            return x >= 0 && x < gridWidth && z >= 0 && z < gridHeight;
        }

        public Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x * cellSize, 0, z * cellSize);
        }

        public GridCell GetCell(int x, int z)
        {
            if (x < 0 || x >= gridWidth || z < 0 || z >= gridHeight)
                return null;

            return grid[x, z];
        }

        public GridCell GetCell(Vector3 worldPosition)
        {
            if (GetGridCoordinates(worldPosition, out int x, out int z))
            {
                return grid[x, z];
            }

            return null;
        }

        public Transform[] GetSpawnPoints()
        {
            return spawnPoints.ToArray();
        }

        public Transform[] GetExitPoints()
        {
            return exitPoints.ToArray();
        }

        public List<Vector3> GetPathFromSpawnPoint(Transform spawnPoint)
        {
            if (pathsFromSpawns.TryGetValue(spawnPoint, out List<Vector3> path))
            {
                return new List<Vector3>(path); // Return copy
            }

            return new List<Vector3>();
        }
        #endregion

        #region Private Methods
        private void ClearGrid()
        {
            if (grid != null)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    for (int z = 0; z < grid.GetLength(1); z++)
                    {
                        if (grid[x, z] != null)
                        {
                            Destroy(grid[x, z].gameObject);
                        }
                    }
                }
            }

            spawnPoints.Clear();
            exitPoints.Clear();
            pathsFromSpawns.Clear();

            if (gridContainer != null)
            {
                foreach (Transform child in gridContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private void CalculatePathsFromSpawnPoints()
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                Transform closestExit = FindClosestExitPoint(spawnPoint.position);
                if (closestExit != null)
                {
                    List<Vector3> path = CalculatePath(
                        GetGridCoordinates(spawnPoint.position, out int startX, out int startZ) ? new Vector2Int(startX, startZ) : Vector2Int.zero,
                        GetGridCoordinates(closestExit.position, out int endX, out int endZ) ? new Vector2Int(endX, endZ) : Vector2Int.zero
                    );

                    pathsFromSpawns[spawnPoint] = path;
                }
            }
        }

        private Transform FindClosestExitPoint(Vector3 position)
        {
            if (exitPoints.Count == 0)
                return null;

            Transform closest = exitPoints[0];
            float closestDistance = Vector3.Distance(position, closest.position);

            for (int i = 1; i < exitPoints.Count; i++)
            {
                float distance = Vector3.Distance(position, exitPoints[i].position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = exitPoints[i];
                }
            }

            return closest;
        }

        private List<Vector3> CalculatePath(Vector2Int start, Vector2Int end)
        {
            // Simple A* implementation could go here
            // For simplicity, we'll use a basic approach
            List<Vector3> path = new List<Vector3>();

            path.Add(GetWorldPosition(start.x, start.y) + Vector3.up * 0.1f);

            List<Vector2Int> pathCells = FindPathCells(start, end);
            foreach (Vector2Int cell in pathCells)
            {
                path.Add(GetWorldPosition(cell.x, cell.y) + Vector3.up * 0.1f);
            }

            path.Add(GetWorldPosition(end.x, end.y) + Vector3.up * 0.1f);

            return path;
        }

        private List<Vector2Int> FindPathCells(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> pathCells = new List<Vector2Int>();

            // For simplicity, just find all path cells and sort by distance to start
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    if (grid[x, z].CellType == CellType.Path)
                    {
                        pathCells.Add(new Vector2Int(x, z));
                    }
                }
            }

            pathCells.Sort((a, b) => Vector2Int.Distance(a, start).CompareTo(Vector2Int.Distance(b, start)));

            return pathCells;
        }
        #endregion
    }
}