using UnityEngine;
using TowerDefense.Core;
using TowerDefense.Grid;
using TowerDefense.Enums;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Handles tower placement and interaction
    /// </summary>
    public class TowerController : MonoBehaviour
    {
        #region Properties
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _gridLayerMask;
        [SerializeField] private TowerFactory _towerFactory;
        [SerializeField] private TowerType _selectedTowerType = TowerType.Basic;
        [SerializeField] private GameObject _placementIndicator;
        
        private bool _isPlacingTower = false;
        private GridCell _highlightedCell = null;
        
        public TowerType SelectedTowerType => _selectedTowerType;
        public bool IsPlacingTower => _isPlacingTower;
        #endregion

        #region Events
        public delegate void TowerSelectedDelegate(TowerType towerType, TowerData towerData);
        public static event TowerSelectedDelegate OnTowerSelected;
        
        public delegate void TowerPlacedDelegate(Tower tower, GridCell cell);
        public static event TowerPlacedDelegate OnTowerPlaced;
        
        public delegate void PlacementModeChangedDelegate(bool isPlacing);
        public static event PlacementModeChangedDelegate OnPlacementModeChanged;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
            
            if (_towerFactory == null)
            {
                _towerFactory = FindObjectOfType<TowerFactory>();
            }
            
            if (_placementIndicator != null)
            {
                _placementIndicator.SetActive(false);
            }
        }
        
        private void Update()
        {
            if (GameManager.Instance.CurrentGameState != GameState.Playing || GameManager.Instance.IsPaused)
                return;
                
            // Tower placement logic
            if (_isPlacingTower)
            {
                HandleTowerPlacement();
            }
            
            // Cancel placement with right-click or escape
            if (_isPlacingTower && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
            {
                CancelPlacement();
            }
        }
        #endregion

        #region Public Methods
        public void SelectTower(TowerType towerType)
        {
            _selectedTowerType = towerType;
            
            TowerData towerData = _towerFactory.GetTowerData(towerType);
            OnTowerSelected?.Invoke(towerType, towerData);
        }
        
        public void StartPlacement()
        {
            if (GameManager.Instance.CurrentGameState != GameState.Playing)
                return;
                
            // Check if player can afford the tower
            TowerData towerData = _towerFactory.GetTowerData(_selectedTowerType);
            if (towerData == null || !ResourceManager.Instance.CanAfford(towerData.Cost))
                return;
                
            _isPlacingTower = true;
            
            if (_placementIndicator != null)
            {
                _placementIndicator.SetActive(true);
            }
            
            OnPlacementModeChanged?.Invoke(true);
        }
        
        public void CancelPlacement()
        {
            _isPlacingTower = false;
            
            if (_placementIndicator != null)
            {
                _placementIndicator.SetActive(false);
            }
            
            // Clear highlighted cell
            if (_highlightedCell != null)
            {
                _highlightedCell.Highlight(false);
                _highlightedCell = null;
            }
            
            OnPlacementModeChanged?.Invoke(false);
        }
        #endregion

        #region Private Methods
        private void HandleTowerPlacement()
        {
            // Raycast to find grid cell
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100f, _gridLayerMask))
            {
                // Get grid cell
                GridCell cell = hit.collider.GetComponent<GridCell>();
                
                if (cell != null)
                {
                    // Clear previous highlight
                    if (_highlightedCell != null && _highlightedCell != cell)
                    {
                        _highlightedCell.Highlight(false);
                    }
                    
                    // Highlight current cell
                    _highlightedCell = cell;
                    
                    // Move placement indicator
                    if (_placementIndicator != null)
                    {
                        _placementIndicator.transform.position = cell.WorldPosition + Vector3.up * 0.1f;
                        
                        // Change color based on placement validity
                        Renderer renderer = _placementIndicator.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            bool canPlace = GridManager.Instance.CanPlaceTower(cell.X, cell.Z);
                            renderer.material.color = canPlace ? Color.green : Color.red;
                        }
                        
                        cell.Highlight(true);
                    }
                    
                    // Place tower on click
                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceTower(cell);
                    }
                }
            }
            else
            {
                // Clear highlighted cell
                if (_highlightedCell != null)
                {
                    _highlightedCell.Highlight(false);
                    _highlightedCell = null;
                }
                
                // Hide placement indicator
                if (_placementIndicator != null)
                {
                    _placementIndicator.transform.position = new Vector3(0, -100, 0); // Move out of view
                }
            }
        }
        
        private void PlaceTower(GridCell cell)
        {
            if (!GridManager.Instance.CanPlaceTower(cell.X, cell.Z))
                return;
                
            // Get tower data
            TowerData towerData = _towerFactory.GetTowerData(_selectedTowerType);
            if (towerData == null)
                return;
                
            // Check if player can afford the tower
            if (!ResourceManager.Instance.SpendCurrency(towerData.Cost))
                return;
                
            // Create tower
            GameObject towerObject = _towerFactory.CreateTower(_selectedTowerType, cell.WorldPosition);
            
            if (towerObject != null)
            {
                // Update grid cell
                GridManager.Instance.PlaceTower(cell.X, cell.Z, towerObject);
                
                // Notify listeners
                Tower tower = towerObject.GetComponent<Tower>();
                if (tower != null)
                {
                    OnTowerPlaced?.Invoke(tower, cell);
                }
                
                // Exit placement mode
                CancelPlacement();
            }
            else
            {
                // Refund cost if tower creation failed
                ResourceManager.Instance.AddCurrency(towerData.Cost);
            }
        }
        #endregion
    }
}