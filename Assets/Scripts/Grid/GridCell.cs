using UnityEngine;

namespace TowerDefense.Grid
{
    /// <summary>
    /// Represents a single cell in the game grid.
    /// </summary>
    public class GridCell : MonoBehaviour
    {
        #region Variables
        [SerializeField] private int x;
        [SerializeField] private int z;
        [SerializeField] private CellType cellType;
        [SerializeField] private Vector3 worldPosition;
        [SerializeField] private GameObject tower;

        private Renderer cellRenderer;
        #endregion

        #region Properties
        public int X => x;
        public int Z => z;
        public CellType CellType => cellType;
        public Vector3 WorldPosition => worldPosition;
        public bool HasTower => tower != null;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            cellRenderer = GetComponent<Renderer>();
        }
        #endregion

        #region Public Methods
        public void Initialize(int x, int z, CellType cellType, Vector3 worldPosition)
        {
            this.x = x;
            this.z = z;
            this.cellType = cellType;
            this.worldPosition = worldPosition;

            UpdateVisuals();
        }

        public void SetTower(GameObject tower)
        {
            this.tower = tower;
        }

        public void RemoveTower()
        {
            tower = null;
        }

        public void Highlight(bool isHighlighted)
        {
            if (cellRenderer != null)
            {
                if (isHighlighted)
                {
                    cellRenderer.material.EnableKeyword("_EMISSION");
                    cellRenderer.material.SetColor("_EmissionColor", Color.white * 0.5f);
                }
                else
                {
                    cellRenderer.material.DisableKeyword("_EMISSION");
                }
            }
        }

        public void UpdateVisuals()
        {
            if (cellRenderer != null)
            {
                switch (cellType)
                {
                    case CellType.Empty:
                        cellRenderer.material.color = new Color(0.8f, 0.8f, 0.8f);
                        break;
                    case CellType.Path:
                        cellRenderer.material.color = new Color(0.6f, 0.4f, 0.2f);
                        break;
                    case CellType.SpawnPoint:
                        cellRenderer.material.color = Color.green;
                        break;
                    case CellType.ExitPoint:
                        cellRenderer.material.color = Color.red;
                        break;
                    case CellType.Obstacle:
                        cellRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);
                        break;
                }
            }
        }
        #endregion
    }
}