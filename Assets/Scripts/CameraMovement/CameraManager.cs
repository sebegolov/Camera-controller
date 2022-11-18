using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;


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
        [SerializeField] private float _speedScale = 2f;
        [SerializeField] private float _minAngle = -0.6f;
        [SerializeField] private float _maxAngle = 0f;

        [Header("Move Bounds")]
        [SerializeField] private Vector2 _minBounds, _maxBounds;

        [Header("Zoom Controls")]
        [SerializeField] private float _zoomSpeed = 4f;
        [SerializeField] private float _nearZoomLimit = 2f;
        [SerializeField] private float _farZoomLimit = 16f;
        [SerializeField] private float _startingZoom = 5f;

        [Header("Turn On/Off Mode")]
        [SerializeField] private bool _onMoving = true;
        [SerializeField] private bool _onRotate = true;
        [SerializeField] private bool _onZoom = true;

        private IZoomBehaviour _zoomBehaviour;
        private Vector3 _frameMove;
        private Vector3 _frameShift;
        private Vector2 _frameRotate;
        private float _frameZoom;
        private Camera _camera;
        private GameObject _rotatingAssembly;

        private GameObject _currentTarget;

        private bool _mouseRotatae = false;
        private bool _obstructionRotatae = false;


        private void Awake()
        {
            _rotatingAssembly = transform.GetChild(0).gameObject;
            _rotatingAssembly.transform.localRotation = Quaternion.Euler(-45,0,0);
            _camera = GetComponentInChildren<Camera>();
            _camera.transform.localPosition = new Vector3(0f, Mathf.Abs(_cameraOffset.y), 0);
            _zoomBehaviour = _camera.orthographic ? (IZoomBehaviour) new OrtographicZoomBehaviour(_camera, _startingZoom) : new PerspectiveZoomBehaviour(_camera, _cameraOffset, _startingZoom);
        }

        private void LateUpdate()
        {
            MoveCamera();

            ObstructionCheck();
            
            RotateCamera();

            ZoomCamera();

            NullFrameParameters();
            LockPositionInBounds();
        }

        #region PublicMethods

        #region SetParameters

        public void SetInOutSpeed(float inOutSpeed)
        {
            _inOutSpeed = inOutSpeed;
        }

        public void SetLateralSpeed(float lateralSpeed)
        {
            _lateralSpeed = lateralSpeed;
        }

        public void SetRotateSpeed(float rotateSpeed)
        {
            _rotateSpeed = rotateSpeed;
        }

        public void SetSpeedScale(float speedScale)
        {
            _speedScale = speedScale;
        }

        public void SetMinAngle(float minAngle)
        {
            _minAngle = minAngle;
        }

        public void SetMaxAngle(float maxAngle)
        {
            _maxAngle = maxAngle;
        }

        public void SetMinBounds(Vector2 minBounds)
        {
            _minBounds = minBounds;
        }

        public void SetMaxBounds(Vector2 maxBounds)
        {
            _maxBounds = maxBounds;
        }

        public void SetZoomSpeed(float zoomSpeed)
        {
            _zoomSpeed = zoomSpeed;
        }

        public void SetNearZoomLimit(float nearZoomLimit)
        {
            _nearZoomLimit = nearZoomLimit;
        }

        public void SetFarZoomLimit(float farZoomLimit)
        {
            _farZoomLimit = farZoomLimit ;
        }

        public void SetStartingZoom(float startingZoom)
        {
            _startingZoom = startingZoom;
        }

        #endregion

        public void SetTarget(GameObject target)
        {
            _currentTarget = target;

            if (_currentTarget != null)
            {
                StartCoroutine(MoveToTarget());
            }
            else
            {
                StopCoroutine(MoveToTarget());
            }
        }

        public void OnMoving()
        {
            _onMoving = true;
        }

        public void OffMoving()
        {
            _onMoving = false;
        }

        public void OnRotation()
        {
            _onRotate = true;
        }

        public void OffRotation()
        {
            _onRotate = false;
        }

        public void OnZoom()
        {
            _onZoom = true;
        }

        public void OffZoom()
        {
            _onZoom = false;
        }

        #endregion

        #region PrivateMethods

        private void ZoomCamera()
        {
            if (!_onZoom) return;
            
            if (_frameZoom < 0f)
            {
                _zoomBehaviour.ZoomIn(_camera, Time.deltaTime * Mathf.Abs(_frameZoom) * _zoomSpeed, _nearZoomLimit);
            }
            else if (_frameZoom > 0f)
            {
                _zoomBehaviour.ZoomOut(_camera, Time.deltaTime * _frameZoom * _zoomSpeed, _farZoomLimit);
            }
        }

        private void RotateCamera()
        {
            if (!_onRotate) return;

            if (_frameRotate != new Vector2() )
            {
                transform.Rotate(Vector3.up, _frameRotate.x * Time.deltaTime * _rotateSpeed);
                _rotatingAssembly.transform.RotateAround(transform.position, transform.right,
                    _frameRotate.y * Time.deltaTime * _rotateSpeed);
            }
        }

        private void MoveCamera()
        {
            if (!_onMoving) return;

            if ((_frameMove != Vector3.zero || _frameShift != Vector3.zero) && !_mouseRotatae)
            {
                Vector3 speedModFrameMove = new Vector3(_frameMove.x * _lateralSpeed, _frameMove.y, _frameMove.z * _inOutSpeed);
                transform.position += transform.TransformDirection(speedModFrameMove) * Time.deltaTime;
                transform.position += _frameShift;
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

        private void Move()
        {
            if (_currentTarget != null)
            {
                Vector3 needPosition = _currentTarget.transform.position;
                needPosition.y = 0;
                transform.position = Vector3.Lerp(transform.position, needPosition,
                    Time.deltaTime);
            }
        }

        private void OnEnable()
        {
            ConnectKeyboardManager();
            ConnectMouseManager();
            ConnectUIManager();
        }

        private void ConnectUIManager()
        {
            UIManager.OnMoveInput += UpdateFrameMove;
            UIManager.OnRotateInput += UpdateFrameRotate;
            UIManager.OnZoomInput += UpdateFrameZoom;
        }

        private void ConnectMouseManager()
        {
            MouseManager.OnMoveInput += UpdateFrameMove;
            MouseManager.OnRotateInput += UpdateFrameRotateMouse;
            MouseManager.OnZoomInput += UpdateFrameZoom;
            MouseManager.OnShiftInput += ShiftCamera;
            MouseManager.OnMouseRotate += SetMouseRotateMode;
        }

        private void ConnectKeyboardManager()
        {
            KeyboardManager.OnMoveInput += UpdateFrameMove;
            KeyboardManager.OnRotateInput += UpdateFrameRotate;
            KeyboardManager.OnZoomInput += UpdateFrameZoom;
            KeyboardManager.OnSpeedScale += ScaleSpeed;
        }

        private void OnDisable()
        {
            DisconnectKeyboardManager();
            DisconnectMouseManager();
            DisconnectUIManager();
        }

        private void DisconnectUIManager()
        {
            UIManager.OnMoveInput -= UpdateFrameMove;
            UIManager.OnRotateInput -= UpdateFrameRotate;
            UIManager.OnZoomInput -= UpdateFrameZoom;
        }

        private void DisconnectMouseManager()
        {
            MouseManager.OnMoveInput -= UpdateFrameMove;
            MouseManager.OnRotateInput -= UpdateFrameRotateMouse;
            MouseManager.OnZoomInput -= UpdateFrameZoom;
            MouseManager.OnShiftInput -= ShiftCamera;
            MouseManager.OnMouseRotate += SetMouseRotateMode;
        }

        private void DisconnectKeyboardManager()
        {
            KeyboardManager.OnMoveInput -= UpdateFrameMove;
            KeyboardManager.OnRotateInput -= UpdateFrameRotate;
            KeyboardManager.OnZoomInput -= UpdateFrameZoom;
            KeyboardManager.OnSpeedScale -= ScaleSpeed;
        }

        private void NullFrameParameters()
        {
            _frameZoom = 0;
            _frameZoom = 0;
            _frameRotate = Vector2.zero;
            _frameMove = Vector3.zero;
            _frameShift = Vector3.zero;
            _obstructionRotatae = false;
        }
        
        private void ObstructionCheck()
        {
            Vector3 CameraPosition = _camera.transform.position;
            Vector3 Position = transform.position;
            Vector3? dotBeforeCamera = null;        // точка перед камерой
            Vector3? dotBehindCamera = null;        // точка за камерой

            float distance = Vector3.Distance(Position, CameraPosition);
            
            RaycastHit hit;
            //check point behind camera
            if (Physics.Raycast(Position, CameraPosition - Position, out hit, distance + 0.5f))
            {
                dotBehindCamera = hit.point;
                _frameZoom -= 20;
            }
            //check point before camera
            if (Physics.Raycast(CameraPosition,Position - CameraPosition,  out hit, distance))
            {
                dotBeforeCamera = hit.point;
                if (Vector3.Distance(CameraPosition, dotBeforeCamera.Value) < 5)
                {
                    _frameZoom += 20;
                }
            }

            float LateralDisplacement = Mathf.Abs(_frameRotate.x);

            if (dotBeforeCamera.HasValue || dotBehindCamera.HasValue)
            {
                if (dotBeforeCamera.HasValue)
                {
                    if (Vector3.Distance(dotBeforeCamera.Value, CameraPosition) < 5)
                    {
                        _frameRotate.y = _frameRotate.y <= 20 ? Mathf.Max(20, LateralDisplacement) :Mathf.Max(_frameRotate.y, LateralDisplacement);
                        _obstructionRotatae = true;
                    }
                }
                if (dotBehindCamera.HasValue)
                {
                    if (Vector3.Distance(dotBehindCamera.Value, CameraPosition) < 5)
                    {
                        _frameRotate.y = _frameRotate.y <= 20 ? Mathf.Max(20, LateralDisplacement) :Mathf.Max(_frameRotate.y, LateralDisplacement);
                        _obstructionRotatae = true;
                    }
                }
            }
            
        }

        private void LockPositionInBounds()
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, _minBounds.x, _maxBounds.x),
                transform.position.y,
                Mathf.Clamp(transform.position.z, _minBounds.y, _maxBounds.y)
            );

            var angles = _rotatingAssembly.transform.localRotation;
            angles.x = Mathf.Clamp(angles.x, _minAngle, _maxAngle);
            
            _rotatingAssembly.transform.localRotation = angles;
        }

        private void UpdateFrameMove(Vector3 moveVector)
        {
            _frameMove += moveVector;
        }

        private void UpdateFrameRotate(Vector2 rotateAmount)
        {
            _frameRotate += rotateAmount;
        }

        private void UpdateFrameRotateMouse(Vector2 rotateAmount)
        {
            if (_mouseRotatae)
            {
                _frameRotate += rotateAmount;
            }
        }

        private void UpdateFrameZoom(float zoomAmount)
        {
            _frameZoom += zoomAmount;
        }

        private void SetMouseRotateMode(bool rotate)
        {
            _mouseRotatae = rotate;
        }

        private void ScaleSpeed()
        {
            _frameMove *= _speedScale;
            _frameRotate *= _speedScale;
            _frameZoom *= _speedScale;
        }

        private void ShiftCamera(Vector3 shift)
        {
            _frameShift += shift;
        }

        #endregion
    }
}