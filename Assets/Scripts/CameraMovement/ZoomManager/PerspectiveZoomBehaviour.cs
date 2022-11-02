using System;
using UnityEngine;

namespace MovementCamera
{
    public class PerspectiveZoomBehaviour : IZoomBehaviour
    {
        private Vector3 _normolizedCameraPosition;
        private float _currentZoomLevel;

        public PerspectiveZoomBehaviour(Camera camera, Vector3 offset, float startingZoom)
        {
            _normolizedCameraPosition = new Vector3(0f, Mathf.Abs(offset.y), -Mathf.Abs(offset.x)).normalized;
            _currentZoomLevel = startingZoom;
            PositonCamera(camera);
        }

        private void PositonCamera(Camera camera)
        {
            camera.transform.localPosition = _normolizedCameraPosition * _currentZoomLevel;
        }

        public void ZoomIn(Camera cam, float delta, float nearZoomLimit)
        {
            if (_currentZoomLevel <= nearZoomLimit) { return; }

            _currentZoomLevel = Mathf.Max(_currentZoomLevel - delta, nearZoomLimit);
            PositonCamera(cam);
        }

        public void ZoomOut(Camera cam, float delta, float farZoomLimit)
        {
            if (_currentZoomLevel >= farZoomLimit) { return; }

            _currentZoomLevel = Mathf.Min(_currentZoomLevel + delta, farZoomLimit);
            PositonCamera(cam);
        }
    }
}
