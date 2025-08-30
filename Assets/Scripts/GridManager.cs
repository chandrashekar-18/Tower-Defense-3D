using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Level;
using TowerDefense.Core;

namespace TowerDefense.Grid
{
    /// <summary>
    /// Manages the game grid and pathfinding
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        #region Singleton
        public static GridManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
        #endregion

        #region Properties
        [SerializeField] private GameObject _gridCellPrefab;
        [SerializeField] private GameObject _pathCellPrefab;
        [SerializeField] private GameObject _spawnPointPrefab;
        [SerializeField] private GameObject _exitPointPrefab;
        
        [SerializeField] private Transform _gridContainer;
        
        [SerializeField] private int _gridWidth = 15;
        [SerializeField] private int _gridHeight = 10;
        [SerializeField] private float _cellSize = 1f;
        
        private GridCell[,] _grid;
        private List<Transform> _spawnPoints = new List<Transform>();
        private List<Transform> _exitPoints = new List<Transform>();
        private Dictionary<Transform, List<Vector3>> _pathsFromSpawns = new Dictionary<Transform, List<Vector3>>();
        
        public int GridWidth => _gridWidth;
        public int GridHeight => _gridHeight;
        public float CellSize => _cellSize;
        #endregion

        #region Events
        public delegate void GridGeneratedDelegate(GridCell[,] grid);
        public event GridGeneratedDelegate OnGridGenerated;
        #endregion

        void Start()
        {
            GenerateGrid(LevelManager.Instance.CurrentLevelData);
        }

        #region Public Methods
        public void GenerateGrid(LevelData levelData)
        {
            ClearGrid();

            // Set grid dimensions from level data
            _gridWidth = levelData.GridWidth;
            _gridHeight = levelData.GridHeight;

            // Create grid container if needed
            if (_gridContainer == null)
            {
                GameObject container = new GameObject("GridContainer");
                _gridContainer = container.transform;
                _gridContainer.position = Vector3.zero;
            }

            // Create grid cells
            _grid = new GridCell[_gridWidth, _gridHeight];

            for (int x = 0; x < _gridWidth; x++)
            {
                for (int z = 0; z < _gridHeight; z++)
                {
                    Vector3 position = new Vector3(x * _cellSize, 0, z * _cellSize);

                    // Determine cell type from level data
                    CellType cellType = levelData.GetCellType(x, z);

                    // Create appropriate cell
                    GameObject cellPrefab = _gridCellPrefab; // Default

                    switch (cellType)
                    {
                        case CellType.Path:
                            cellPrefab = _pathCellPrefab;
                            break;
                        case CellType.SpawnPoint:
                            cellPrefab = _spawnPointPrefab;
                            break;
                        case CellType.ExitPoint:
                            cellPrefab = _exitPointPrefab;
                            break;
                    }

                    GameObject cellObject = Instantiate(cellPrefab, position, Quaternion.identity, _gridContainer);
                    cellObject.name = $"Cell_{x}_{z}";

                    GridCell cell = cellObject.GetComponent<GridCell>();
                    if (cell == null)
                    {
                        cell = cellObject.AddComponent<GridCell>();
                    }

                    cell.Initialize(x, z, cellType, position);
                    _grid[x, z] = cell;

                    // Track spawn and exit points
                    if (cellType == CellType.SpawnPoint)
                    {
                        _spawnPoints.Add(cellObject.transform);
                    }
                    else if (cellType == CellType.ExitPoint)
                    {
                        _exitPoints.Add(cellObject.transform);
                    }
                }
            }

            // Calculate paths from spawn points
            CalculatePathsFromSpawnPoints();

            // Notify subscribers
            OnGridGenerated?.Invoke(_grid);
        }
        
        public bool CanPlaceTower(int x, int z)
        {
            if (x < 0 || x >= _gridWidth || z < 0 || z >= _gridHeight)
                return false;
                
            return _grid[x, z].CellType == CellType.Empty && !_grid[x, z].HasTower;
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
                
            Vector3 position = _grid[x, z].transform.position;
            position.y += _cellSize / 2; // Adjust height
            
            tower.transform.position = position;
            _grid[x, z].SetTower(tower);
            
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
            x = Mathf.FloorToInt(worldPosition.x / _cellSize);
            z = Mathf.FloorToInt(worldPosition.z / _cellSize);
            
            return x >= 0 && x < _gridWidth && z >= 0 && z < _gridHeight;
        }
        
        public Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x * _cellSize, 0, z * _cellSize);
        }
        
        public GridCell GetCell(int x, int z)
        {
            if (x < 0 || x >= _gridWidth || z < 0 || z >= _gridHeight)
                return null;
                
            return _grid[x, z];
        }
        
        public GridCell GetCell(Vector3 worldPosition)
        {
            if (GetGridCoordinates(worldPosition, out int x, out int z))
            {
                return _grid[x, z];
            }
            
            return null;
        }
        
        public Transform[] GetSpawnPoints()
        {
            return _spawnPoints.ToArray();
        }
        
        public Transform[] GetExitPoints()
        {
            return _exitPoints.ToArray();
        }
        
        public List<Vector3> GetPathFromSpawnPoint(Transform spawnPoint)
        {
            if (_pathsFromSpawns.TryGetValue(spawnPoint, out List<Vector3> path))
            {
                return new List<Vector3>(path); // Return copy
            }
            
            return new List<Vector3>();
        }
        #endregion

        #region Private Methods
        private void ClearGrid()
        {
            // Clear existing grid
            if (_grid != null)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    for (int z = 0; z < _grid.GetLength(1); z++)
                    {
                        if (_grid[x, z] != null)
                        {
                            Destroy(_grid[x, z].gameObject);
                        }
                    }
                }
            }
            
            // Clear lists
            _spawnPoints.Clear();
            _exitPoints.Clear();
            _pathsFromSpawns.Clear();
            
            // Clear container
            if (_gridContainer != null)
            {
                foreach (Transform child in _gridContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        
        private void CalculatePathsFromSpawnPoints()
        {
            foreach (Transform spawnPoint in _spawnPoints)
            {
                // Find closest exit point and calculate path
                Transform closestExit = FindClosestExitPoint(spawnPoint.position);
                if (closestExit != null)
                {
                    List<Vector3> path = CalculatePath(
                        GetGridCoordinates(spawnPoint.position, out int startX, out int startZ) ? new Vector2Int(startX, startZ) : Vector2Int.zero,
                        GetGridCoordinates(closestExit.position, out int endX, out int endZ) ? new Vector2Int(endX, endZ) : Vector2Int.zero
                    );
                    
                    _pathsFromSpawns[spawnPoint] = path;
                }
            }
        }
        
        private Transform FindClosestExitPoint(Vector3 position)
        {
            if (_exitPoints.Count == 0)
                return null;
                
            Transform closest = _exitPoints[0];
            float closestDistance = Vector3.Distance(position, closest.position);
            
            for (int i = 1; i < _exitPoints.Count; i++)
            {
                float distance = Vector3.Distance(position, _exitPoints[i].position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = _exitPoints[i];
                }
            }
            
            return closest;
        }
        
        private List<Vector3> CalculatePath(Vector2Int start, Vector2Int end)
        {
            // Simple A* implementation could go here
            // For simplicity, we'll use a basic approach
            
            List<Vector3> path = new List<Vector3>();
            
            // Start with spawn point
            path.Add(GetWorldPosition(start.x, start.y) + Vector3.up * 0.1f);
            
            // Find all path cells and add them in sequence
            List<Vector2Int> pathCells = FindPathCells(start, end);
            foreach (Vector2Int cell in pathCells)
            {
                path.Add(GetWorldPosition(cell.x, cell.y) + Vector3.up * 0.1f);
            }
            
            // Add end point
            path.Add(GetWorldPosition(end.x, end.y) + Vector3.up * 0.1f);
            
            return path;
        }
        
        private List<Vector2Int> FindPathCells(Vector2Int start, Vector2Int end)
        {
            // Find all cells of type Path between start and end
            List<Vector2Int> pathCells = new List<Vector2Int>();
            
            // For simplicity, just find all path cells and sort by distance to start
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int z = 0; z < _gridHeight; z++)
                {
                    if (_grid[x, z].CellType == CellType.Path)
                    {
                        pathCells.Add(new Vector2Int(x, z));
                    }
                }
            }
            
            // Sort by distance to start
            pathCells.Sort((a, b) => 
                Vector2Int.Distance(a, start).CompareTo(Vector2Int.Distance(b, start))
            );
            
            return pathCells;
        }
        #endregion
    }
}