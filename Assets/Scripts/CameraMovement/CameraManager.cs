using System;
using System.Collections;
using UnityEngine;


namespace MovementCamera
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera Positioning")]
        [SerializeField] private Vector2 _cameraOffset = new Vector2(10f, 14f);
        [SerializeField] private float _lookAtOffset = 2f;

        [Header("Move Controls")]
        [SerializeField] private float _inOutSpeed = 5f;
        [SerializeField] private float _lateralSpeed = 5f;
        [SerializeField] private float _rotateSpeed = 5f;

        [Header("Move Bounds")]
        [SerializeField] private Vector2 _minBounds, _maxBounds;

        [Header("Zoom Controls")]
        [SerializeField] private float _zoomSpeed = 4f;
        [SerializeField] private float _nearZoomLimit = 2f;
        [SerializeField] private float _farZoomLimit = 16f;
        [SerializeField] private float _startingZoom = 5f;

        private IZoomBehaviour _zoomBehaviour;
        private Vector3 _frameMove;
        private float _frameRotate;
        private float _frameZoom;
        private Camera _camera;

        [SerializeField] private GameObject[] _testList;
        private GameObject _currentTarget;


        private void Awake()
        {
            _camera = GetComponentInChildren<Camera>();
            _camera.transform.localPosition = new Vector3(0f, Mathf.Abs(_cameraOffset.y), - Mathf.Abs(_cameraOffset.x));
            _zoomBehaviour = _camera.orthographic ? (IZoomBehaviour) new OrtographicZoomBehaviour(_camera, _startingZoom) : new PerspectiveZoomBehaviour(_camera, _cameraOffset, _startingZoom);
            _camera.transform.LookAt(transform.position + Vector3.up * _lookAtOffset);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F1))
            {
                SetTarget(_testList[0]);
            }
            if (Input.GetKeyUp(KeyCode.F2))
            {
                SetTarget(_testList[1]);
            }
            if (Input.GetKeyUp(KeyCode.F3))
            {
                SetTarget(_testList[2]);
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                SetTarget(null);
            }
        }

        private void SetTarget(GameObject target)
        {
            _currentTarget = target;

            if (_currentTarget != null)
            {
                StartCoroutine(MoveToTarget());
            }
        }
        
        IEnumerator MoveToTarget()
        {
            while (_currentTarget && transform.position != _currentTarget.transform.position)
            {
                Move();
                yield return null;
           }
        }

        public void Move()
        {
            if (_currentTarget != null)
                transform.position = Vector3.Lerp(transform.position, _currentTarget.transform.position,
                    Time.deltaTime);
        }


        private void OnEnable()
        {
            KeyboardManager.OnMoveInput += UpdateFrameMove;
            KeyboardManager.OnRotateInput += UpdateFrameRotate;
            KeyboardManager.OnZoomInput += UpdateFrameZoom;
            
            MouseManager.OnMoveInput += UpdateFrameMove;
            MouseManager.OnRotateInput += UpdateFrameRotate;
            MouseManager.OnZoomInput += UpdateFrameZoom;
            
            UIManager.OnMoveInput += UpdateFrameMove;
            UIManager.OnRotateInput += UpdateFrameRotate;
            UIManager.OnZoomInput += UpdateFrameZoom;
        }
        
        private void OnDisable()
        {
            KeyboardManager.OnMoveInput -= UpdateFrameMove;
            KeyboardManager.OnRotateInput -= UpdateFrameRotate;
            KeyboardManager.OnZoomInput -= UpdateFrameZoom;
            
            MouseManager.OnMoveInput -= UpdateFrameMove;
            MouseManager.OnRotateInput -= UpdateFrameRotate;
            MouseManager.OnZoomInput -= UpdateFrameZoom;
            
            UIManager.OnMoveInput -= UpdateFrameMove;
            UIManager.OnRotateInput -= UpdateFrameRotate;
            UIManager.OnZoomInput -= UpdateFrameZoom;
        }

        private void UpdateFrameMove(Vector3 moveVector)
        {
            _frameMove += moveVector;
        }
        
        private void UpdateFrameRotate(float rotateAmount)
        {
            _frameRotate += rotateAmount;
        }

        private void UpdateFrameZoom(float zoomAmount)
        {
            _frameZoom += zoomAmount;
        }

        private void LateUpdate()
        {
            if (_frameMove != Vector3.zero)
            {
                Vector3 speedModFrameMove = new Vector3(_frameMove.x * _lateralSpeed, _frameMove.y, _frameMove.z * _inOutSpeed);
                transform.position += transform.TransformDirection(speedModFrameMove) * Time.deltaTime;
                LockPositionInBounds();
                _frameMove = Vector3.zero;
            }

            if (_frameRotate != 0f)
            {
                transform.Rotate(Vector3.up, _frameRotate * Time.deltaTime * _rotateSpeed);
                _frameRotate = 0f;
            }

            if (_frameZoom < 0f)
            {
                _zoomBehaviour.ZoomIn(_camera, Time.deltaTime * Mathf.Abs(_frameZoom) * _zoomSpeed, _nearZoomLimit);
                _frameZoom = 0;
            }
            else if (_frameZoom > 0f)
            {
                _zoomBehaviour.ZoomOut(_camera, Time.deltaTime *_frameZoom * _zoomSpeed, _farZoomLimit);
                _frameZoom = 0;
            }
        }

        private void LockPositionInBounds()
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, _minBounds.x, _maxBounds.x),
                transform.position.y,
                Mathf.Clamp(transform.position.z, _minBounds.y, _maxBounds.y)
                );
        }
    }
}
