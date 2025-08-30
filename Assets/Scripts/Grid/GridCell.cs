using UnityEngine;

namespace TowerDefense.Grid
{
    /// <summary>
    /// Represents a single cell in the game grid
    /// </summary>
    public class GridCell : MonoBehaviour
    {
        #region Properties
        [SerializeField] private int _x;
        [SerializeField] private int _z;
        [SerializeField] private CellType _cellType;
        [SerializeField] private Vector3 _worldPosition;
        [SerializeField] private GameObject _tower;

        private Renderer _renderer;

        public int X => _x;
        public int Z => _z;
        public CellType CellType => _cellType;
        public Vector3 WorldPosition => _worldPosition;
        public bool HasTower => _tower != null;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }
        #endregion

        #region Public Methods
        public void Initialize(int x, int z, CellType cellType, Vector3 worldPosition)
        {
            _x = x;
            _z = z;
            _cellType = cellType;
            _worldPosition = worldPosition;

            UpdateVisuals();
        }

        public void SetTower(GameObject tower)
        {
            _tower = tower;
        }

        public void RemoveTower()
        {
            _tower = null;
        }

        public void Highlight(bool isHighlighted)
        {
            if (_renderer != null)
            {
                if (isHighlighted)
                {
                    _renderer.material.EnableKeyword("_EMISSION");
                    _renderer.material.SetColor("_EmissionColor", Color.white * 0.5f);
                }
                else
                {
                    _renderer.material.DisableKeyword("_EMISSION");
                }
            }
        }

        public void UpdateVisuals()
        {
            // Update cell visuals based on type
            if (_renderer != null)
            {
                switch (_cellType)
                {
                    case CellType.Empty:
                        _renderer.material.color = new Color(0.8f, 0.8f, 0.8f);
                        break;
                    case CellType.Path:
                        _renderer.material.color = new Color(0.6f, 0.4f, 0.2f);
                        break;
                    case CellType.SpawnPoint:
                        _renderer.material.color = Color.green;
                        break;
                    case CellType.ExitPoint:
                        _renderer.material.color = Color.red;
                        break;
                    case CellType.Obstacle:
                        _renderer.material.color = new Color(0.3f, 0.3f, 0.3f);
                        break;
                }
            }
        }
        #endregion
    }

}