using PerspectiveCameraWall.Core;
using UnityEngine;

namespace PerspectiveCameraWall.Sample2
{
    public class SampleCinemachineInfo : ICameraInfo
    {
        public Camera MainCamera => SampleCinemachineManager.Instance.mainCamera;
        
        public Vector3 MainCameraTargetPosition => SampleCinemachineManager.Instance.target.transform.position;
    }
}