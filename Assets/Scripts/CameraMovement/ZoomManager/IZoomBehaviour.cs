using UnityEngine;

namespace MovementCamera
{
    public interface IZoomBehaviour
    {
        void ZoomIn(Camera cam, float delta, float nearZoomLimit);
        void ZoomOut(Camera cam, float delta, float farZoomLimit);
    }
}
