using UnityEngine;
using PerspectiveCameraWall.Core;

namespace PerspectiveCameraWall.Sample1
{
    public class SampleCameraManager : MonoBehaviour
    {
        public Camera mainCamera;

        public Vector3 offset;

        public GameObject target;

        public bool useCameraWall = true;
    
        public static SampleCameraManager Instance { get; private set; }

        #region Unity Event
        private void Awake()
        {
            Instance = this;

            CameraWall.SetInfo(new SampleCameraInfo());
        }
    
        private void Update()
        {
            mainCamera.transform.position = target.transform.position + offset;

            if (useCameraWall)
            {
                mainCamera.transform.Translate(CameraWall.GetTotalOffset(), Space.World);
            }
        }

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 30;
            GUI.Label(new Rect(0, 0, 400, 100), $"Use CameraWall: { useCameraWall }");
        }
        #endregion
    }
}
