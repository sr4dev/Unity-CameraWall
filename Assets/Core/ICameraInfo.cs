using UnityEngine;

namespace PerspectiveCameraWall.Core
{
    public interface ICameraInfo
    {
        Camera MainCamera { get; }
        
        Vector3 MainCameraTargetPosition { get; }
    }
}