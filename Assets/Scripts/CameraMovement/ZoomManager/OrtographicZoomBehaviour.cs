using System;
using UnityEngine;

namespace MovementCamera
{
    public class OrtographicZoomBehaviour : IZoomBehaviour
    {
        public OrtographicZoomBehaviour(Camera camera, float startingZoom)
        {
            camera.orthographicSize = startingZoom;
        }
        
        public void ZoomIn(Camera cam, float delta, float nearZoomLimit)
        {
            if (cam.orthographicSize == nearZoomLimit) return;

            cam.orthographicSize = Mathf.Max(cam.orthographicSize - delta, nearZoomLimit);
        }

        public void ZoomOut(Camera cam, float delta, float farZoomLimit)
        {
            if (cam.orthographicSize == farZoomLimit) return;

            cam.orthographicSize = Mathf.Max(cam.orthographicSize + delta, farZoomLimit);
        }
    }
}
