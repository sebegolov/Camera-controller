using System;
using UnityEngine;


namespace MovementCamera
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera Positioning")]
        public Vector2 cameraOffset = new Vector2(10f, 14f);
        public float lookAtOffset = 2f;

        [Header("Move Controls")]
        public float inOutSpeed = 5f;
        public float lateralSpeed = 5f;
        public float rotateSpeed = 5f;

        [Header("Move Bounds")]
        public Vector2 minBounds, maxBounds;

        [Header("Zoom Controls")]
        public float zoomSpeed = 4f;
        public float nearZoomLimit = 2f;
        public float farZoomLimit = 16f;
        public float startingZoom = 5f;

        private IZoomBehaviour _zoomBehaviour;
        private Vector3 _frameMove;
        private float _frameRotate;
        private float _frameZoom;
        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponentInChildren<Camera>();
            _camera.transform.localPosition = new Vector3(0f, Mathf.Abs(cameraOffset.y), - Mathf.Abs(cameraOffset.x));
            _zoomBehaviour = _camera.orthographic ? (IZoomBehaviour) new OrtographicZoomBehaviour(_camera, startingZoom) : new PerspectiveZoomBehaviour(_camera, cameraOffset, startingZoom);
            _camera.transform.LookAt(transform.position + Vector3.up * lookAtOffset);
        }

        private void OnEnable()
        {
            KeyboardManager.OnMoveInput += UpdateFrameMove;
            KeyboardManager.OnRotateInput += UpdateFrameRotate;
            KeyboardManager.OnZoomInput += UpdateFrameZoom;
            MouseManager.OnMoveInput += UpdateFrameMove;
            MouseManager.OnRotateInput += UpdateFrameRotate;
            MouseManager.OnZoomInput += UpdateFrameZoom;
        }
        
        private void OnDisable()
        {
            KeyboardManager.OnMoveInput -= UpdateFrameMove;
            KeyboardManager.OnRotateInput -= UpdateFrameRotate;
            KeyboardManager.OnZoomInput -= UpdateFrameZoom;
            MouseManager.OnMoveInput -= UpdateFrameMove;
            MouseManager.OnRotateInput -= UpdateFrameRotate;
            MouseManager.OnZoomInput -= UpdateFrameZoom;
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
                Vector3 speedModFrameMove = new Vector3(_frameMove.x * lateralSpeed, _frameMove.y, _frameMove.z * inOutSpeed);
                transform.position += transform.TransformDirection(speedModFrameMove) * Time.deltaTime;
                LockPositionInBounds();
                _frameMove = Vector3.zero;
            }

            if (_frameRotate != 0f)
            {
                transform.Rotate(Vector3.up, _frameRotate * Time.deltaTime * rotateSpeed);
                _frameRotate = 0f;
            }

            if (_frameZoom < 0f)
            {
                _zoomBehaviour.ZoomIn(_camera, Time.deltaTime * Mathf.Abs(_frameZoom) * zoomSpeed, nearZoomLimit);
                _frameZoom = 0;
            }
            else if (_frameZoom > 0f)
            {
                _zoomBehaviour.ZoomOut(_camera, Time.deltaTime *_frameZoom * zoomSpeed, farZoomLimit);
                _frameZoom = 0;
            }
        }

        private void LockPositionInBounds()
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                transform.position.y,
                Mathf.Clamp(transform.position.z, minBounds.y, maxBounds.y)
                );
        }
    }
}
