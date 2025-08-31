using UnityEngine;
using TowerDefense.Core;
using TowerDefense.Grid;
using TowerDefense.Enums;

namespace TowerDefense.Towers
{
    /// <summary>
    /// Handles tower placement and interaction.
    /// </summary>
    public class TowerController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask gridLayerMask;
        [SerializeField] private TowerFactory towerFactory;
        [SerializeField] private TowerType selectedTowerType = TowerType.Basic;
        [SerializeField] private GameObject placementIndicator;

        private bool isPlacingTower = false;
        private GridCell highlightedCell = null;
        #endregion

        #region Properties
        public TowerType SelectedTowerType => selectedTowerType;
        public bool IsPlacingTower => isPlacingTower;
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
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (towerFactory == null)
            {
                towerFactory = FindObjectOfType<TowerFactory>();
            }

            if (placementIndicator != null)
            {
                placementIndicator.SetActive(false);
            }
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentGameState != GameState.Playing || GameManager.Instance.IsPaused)
                return;

            // Tower placement logic
            if (isPlacingTower)
            {
                HandleTowerPlacement();
            }

            // Cancel placement with right-click or escape
            if (isPlacingTower && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
            {
                CancelPlacement();
            }
        }
        #endregion

        #region Public Methods
        public void SelectTower(TowerType towerType)
        {
            selectedTowerType = towerType;

            TowerData towerData = towerFactory.GetTowerData(towerType);
            OnTowerSelected?.Invoke(towerType, towerData);
        }

        public void StartPlacement()
        {
            if (GameManager.Instance.CurrentGameState != GameState.Playing)
                return;

            // Check if player can afford the tower
            TowerData towerData = towerFactory.GetTowerData(selectedTowerType);
            if (towerData == null || !ResourceManager.Instance.CanAfford(towerData.Cost))
                return;

            isPlacingTower = true;

            if (placementIndicator != null)
            {
                placementIndicator.SetActive(true);
            }

            OnPlacementModeChanged?.Invoke(true);
        }

        public void CancelPlacement()
        {
            isPlacingTower = false;

            if (placementIndicator != null)
            {
                placementIndicator.SetActive(false);
            }

            // Clear highlighted cell
            if (highlightedCell != null)
            {
                highlightedCell.Highlight(false);
                highlightedCell = null;
            }

            OnPlacementModeChanged?.Invoke(false);
        }
        #endregion

        #region Private Methods
        private void HandleTowerPlacement()
        {
            // Raycast to find grid cell
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, gridLayerMask))
            {
                // Get grid cell
                GridCell cell = hit.collider.GetComponent<GridCell>();

                if (cell != null)
                {
                    // Clear previous highlight
                    if (highlightedCell != null && highlightedCell != cell)
                    {
                        highlightedCell.Highlight(false);
                    }

                    // Highlight current cell
                    highlightedCell = cell;

                    // Move placement indicator
                    if (placementIndicator != null)
                    {
                        placementIndicator.transform.position = cell.WorldPosition + Vector3.up * 0.1f;

                        // Change color based on placement validity
                        Renderer renderer = placementIndicator.GetComponent<Renderer>();
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
                if (highlightedCell != null)
                {
                    highlightedCell.Highlight(false);
                    highlightedCell = null;
                }

                // Hide placement indicator
                if (placementIndicator != null)
                {
                    placementIndicator.transform.position = new Vector3(0, -100, 0); // Move out of view
                }
            }
        }

        private void PlaceTower(GridCell cell)
        {
            if (!GridManager.Instance.CanPlaceTower(cell.X, cell.Z))
                return;

            // Get tower data
            TowerData towerData = towerFactory.GetTowerData(selectedTowerType);
            if (towerData == null)
                return;

            // Check if player can afford the tower
            if (!ResourceManager.Instance.SpendCurrency(towerData.Cost))
                return;

            // Create tower
            GameObject towerObject = towerFactory.CreateTower(selectedTowerType, cell.WorldPosition);

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