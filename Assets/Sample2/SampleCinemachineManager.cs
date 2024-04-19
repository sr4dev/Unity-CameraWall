using UnityEngine;
using PerspectiveCameraWall.Core;

namespace PerspectiveCameraWall.Sample2
{
    public class SampleCinemachineManager : MonoBehaviour
    {
        public Camera mainCamera;

        public Vector3 offset;

        public GameObject target;

        public bool useCameraWall = true;
    
        public static SampleCinemachineManager Instance { get; private set; }

        #region Unity Event
        private void Awake()
        {
            Instance = this;

            CameraWall.SetInfo(new SampleCinemachineInfo());
        }
    
        // private void Update()
        // {
        //     mainCamera.transform.position = target.transform.position + offset;
        //
        //     if (useCameraWall && CameraWall.TryGetClosestWall(out var cameraWall))
        //     {
        //         mainCamera.transform.Translate(cameraWall.CalculateOffset(), Space.World);
        //     }
        // }

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 30;
            GUI.Label(new Rect(0, 0, 400, 100), $"Use CameraWall: { useCameraWall }");
        }
        #endregion
    }
}
