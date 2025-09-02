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
        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private LayerMask gridLayerMask;
        [SerializeField] private TowerFactory towerFactory;
        [SerializeField] private string selectedTowerID;
        [Header("Touch Settings")]
        [SerializeField] private float dragThreshold = 0.1f;
        private Vector2 touchStartPosition;
        private bool isDragging = false;
        private bool canPlace = false;
        private bool isPlacingTower = false;
        private GridCell highlightedCell = null;
        private TowerData selectedTowerData;
        #endregion

        #region Properties
        public string SelectedTowerID => selectedTowerID;
        public TowerData SelectedTowerData => selectedTowerData;
        public bool IsPlacingTower => isPlacingTower;
        #endregion

        #region Events
        public delegate void TowerSelectedDelegate(string towerID, TowerData towerData);
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
                mainCamera = UnityEngine.Camera.main;
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
        public void SelectTower(string towerID)
        {
            selectedTowerID = towerID;
            selectedTowerData = towerFactory.GetTowerData(towerID);

            OnTowerSelected?.Invoke(towerID, selectedTowerData);
        }

        private void HandleTowerPlacement()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleDesktopPlacement();
#else
        HandleMobilePlacement();
#endif
        }

        private void HandleDesktopPlacement()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, gridLayerMask))
            {
                HandleCellHover(hit);

                // Place tower on mouse up after dragging
                if (Input.GetMouseButtonUp(0) && isDragging && canPlace)
                {
                    PlaceTower(highlightedCell);
                    isDragging = false;
                }
            }
            else
            {
                ClearHighlight();
            }

            // Start dragging on mouse down
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPosition = Input.mousePosition;
                isDragging = true;
            }
        }

        private void HandleMobilePlacement()
        {
            if (Input.touchCount == 0)
            {
                ClearHighlight();
                return;
            }

            Touch touch = Input.GetTouch(0);
            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPosition = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging && Physics.Raycast(ray, out hit, 100f, gridLayerMask))
                    {
                        HandleCellHover(hit);
                    }
                    break;

                case TouchPhase.Ended:
                    if (isDragging && canPlace && highlightedCell != null)
                    {
                        PlaceTower(highlightedCell);
                    }
                    isDragging = false;
                    break;
            }
        }

        private void HandleCellHover(RaycastHit hit)
        {
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
                canPlace = GridManager.Instance.CanPlaceTower(cell.X, cell.Z);

                cell.Highlight(true);
            }
        }

        private void ClearHighlight()
        {
            if (highlightedCell != null)
            {
                highlightedCell.Highlight(false);
                highlightedCell = null;
            }

            canPlace = false;
        }

        public void StartPlacement()
        {
            if (GameManager.Instance.CurrentGameState != GameState.Playing)
                return;

            if (selectedTowerData == null || !CurrencyManager.Instance.CanAfford(selectedTowerData.Cost))
                return;

            isPlacingTower = true;
            isDragging = false;
            canPlace = false;
            OnPlacementModeChanged?.Invoke(true);
        }

        public void CancelPlacement()
        {
            isPlacingTower = false;

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
        private void PlaceTower(GridCell cell)
        {
            if (!GridManager.Instance.CanPlaceTower(cell.X, cell.Z))
                return;

            if (selectedTowerData == null || !CurrencyManager.Instance.SpendCurrency(selectedTowerData.Cost))
                return;

            GameObject towerObject = towerFactory.CreateTower(selectedTowerData, cell.WorldPosition);

            if (towerObject != null)
            {
                GridManager.Instance.PlaceTower(cell.X, cell.Z, towerObject);

                Tower tower = towerObject.GetComponent<Tower>();
                if (tower != null)
                {
                    OnTowerPlaced?.Invoke(tower, cell);
                }

                CancelPlacement();
            }
            else
            {
                CurrencyManager.Instance.AddCurrency(selectedTowerData.Cost);
            }
        }
        #endregion
    }
}