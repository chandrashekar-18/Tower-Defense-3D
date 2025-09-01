using UnityEngine;
using TowerDefense.Grid;

namespace TowerDefense.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private bool isInputEnabled = true;
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float zoomSpeed = 4f;
        [SerializeField] private float minZoom = 10f;
        [SerializeField] private float maxZoom = 40f;
        [SerializeField] private float defaultAngle = 45f;
        
        [Header("Mobile Settings")]
        [SerializeField] private float pinchZoomSpeed = 0.5f;
        [SerializeField] private float touchMoveSensitivity = 0.5f;
        
        [Header("Boundaries")]
        [SerializeField] private float boundaryPadding = 5f;
        
        private Camera mainCamera;
        private Transform cameraTransform;
        private GridManager gridManager;
        private Vector3 targetPosition;
        private float currentZoom;
        
        private float minX, maxX, minZ, maxZ;
        private Vector2? lastTouchPosition;
        private float initialTouchDistance;
        private bool isDragging = false;
        
        private void Start()
        {
            mainCamera = Camera.main;
            cameraTransform = mainCamera.transform;
            gridManager = GridManager.Instance;
            
            SetupInitialPosition();
            SetBoundaries();
        }
        
        private void SetupInitialPosition()
        {
            float gridWidth = gridManager.GridWidth * gridManager.CellSize;
            float gridLength = gridManager.GridHeight * gridManager.CellSize;
            Vector3 gridCenter = new Vector3(gridWidth * 0.5f, 0f, gridLength * 0.5f);
            
            currentZoom = Mathf.Clamp(Mathf.Max(gridWidth, gridLength) * 0.8f, minZoom, maxZoom);
            
            targetPosition = gridCenter;
            cameraTransform.position = targetPosition + Quaternion.Euler(defaultAngle, 0, 0) * Vector3.back * currentZoom;
            cameraTransform.rotation = Quaternion.Euler(defaultAngle, 0, 0);
        }
        
        private void SetBoundaries()
        {
            float gridWidth = gridManager.GridWidth * gridManager.CellSize;
            float gridLength = gridManager.GridHeight * gridManager.CellSize;

            minX = -boundaryPadding;
            maxX = gridWidth + boundaryPadding;
            minZ = -boundaryPadding;
            maxZ = gridLength + boundaryPadding;
        }
        
        private void Update()
        {
            if (!isInputEnabled) return;

            #if UNITY_EDITOR || UNITY_STANDALONE
            HandleDesktopInput();
            #else
                HandleMobileInput();
            #endif
            
            UpdateCameraPosition();
        }
        
        private void HandleDesktopInput()
        {
            // Keyboard movement
            Vector3 moveDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                moveDirection += Vector3.forward;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                moveDirection += Vector3.back;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                moveDirection += Vector3.left;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                moveDirection += Vector3.right;
            
            targetPosition += moveDirection.normalized * moveSpeed * Time.deltaTime;
            
            // Mouse drag
            if (Input.GetMouseButtonDown(2))
            {
                isDragging = true;
                lastTouchPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(2))
            {
                isDragging = false;
                lastTouchPosition = null;
            }
            
            if (isDragging && lastTouchPosition.HasValue)
            {
                Vector2 delta = (Vector2)Input.mousePosition - lastTouchPosition.Value;
                targetPosition += new Vector3(-delta.x, 0, -delta.y) * moveSpeed * Time.deltaTime * 0.01f;
                lastTouchPosition = Input.mousePosition;
            }
            
            // Mouse wheel zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            currentZoom -= scroll * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
        
        private void HandleMobileInput()
        {
            if (Input.touchCount == 0)
            {
                lastTouchPosition = null;
                return;
            }
            
            // Single touch pan
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                
                if (touch.phase == TouchPhase.Began)
                {
                    lastTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved && lastTouchPosition.HasValue)
                {
                    Vector2 delta = touch.position - lastTouchPosition.Value;
                    targetPosition += new Vector3(-delta.x, 0, -delta.y) * touchMoveSensitivity * Time.deltaTime;
                    lastTouchPosition = touch.position;
                }
            }
            // Pinch to zoom
            else if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);
                
                if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
                {
                    initialTouchDistance = Vector2.Distance(touch0.position, touch1.position);
                }
                else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
                {
                    float currentTouchDistance = Vector2.Distance(touch0.position, touch1.position);
                    float delta = (currentTouchDistance - initialTouchDistance) * pinchZoomSpeed;
                    
                    currentZoom -= delta * Time.deltaTime;
                    currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
                    
                    initialTouchDistance = currentTouchDistance;
                }
            }
        }
        
        private void UpdateCameraPosition()
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);
            
            Vector3 newPosition = targetPosition + Quaternion.Euler(defaultAngle, 0, 0) * Vector3.back * currentZoom;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, newPosition, Time.deltaTime * 10f);
        }
    }
}