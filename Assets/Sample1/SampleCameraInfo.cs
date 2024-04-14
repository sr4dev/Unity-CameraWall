using PerspectiveCameraWall.Core;
using UnityEngine;

namespace PerspectiveCameraWall.Sample1
{
    public class SampleCameraInfo : ICameraInfo
    {
        public Camera MainCamera => SampleCameraManager.Instance.mainCamera;
        
        public Vector3 MainCameraTargetPosition => SampleCameraManager.Instance.target.transform.position;
    }
}