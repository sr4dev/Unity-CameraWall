using PerspectiveCameraWall.Core;
using UnityEngine;

namespace Example.BuiltInCamera
{
    public class CameraInfo : ICameraInfo
    {
        public Camera MainCamera => CameraManager.Instance.mainCamera;
        
        public Vector3 MainCameraTargetPosition => CameraManager.Instance.target.transform.position;
    }
}