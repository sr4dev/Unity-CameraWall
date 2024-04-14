using System.Collections.Generic;
using UnityEngine;

namespace PerspectiveCameraWall.Core
{
    public class CameraWall : MonoBehaviour
    {
        protected static List<CameraWall> _walls = new List<CameraWall>(100);
        protected static ICameraInfo _cameraInfo;
        
        public float distanceZ = 1.0f;
        public float blendDistanceZ = 5.0f;

        #region Unity Event
        /// <summary>
        /// Support for Enter Play Mode
        /// https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected static void RunOnStart()
        {
            _walls.Clear();
            _cameraInfo = default;
        }

        protected virtual void OnEnable()
        {
            _walls.Add(this);
        }

        protected virtual void OnDisable()
        {
            _walls.Remove(this);
        }
        #endregion
        
        public static void SetInfo<T>(T cameraInfo) where T : ICameraInfo, new()
        {
            _cameraInfo = cameraInfo;
        }
        
        public static bool TryGetClosestWall(out CameraWall closestWall)
        {
            closestWall = null;

            foreach (CameraWall wall in _walls)
            {
                if (wall.IsVisible() == false)
                {
                    continue;
                }
                
                if (closestWall == null || closestWall.GetTargetDiff().sqrMagnitude > wall.GetTargetDiff().sqrMagnitude)
                {
                    closestWall = wall;
                }
            }

            return closestWall != null;
        }

        public virtual Vector3 CalculateOffset()
        {
            Camera mainCamera = _cameraInfo.MainCamera;
            Vector3 wallPoint = transform.position;
            Vector3 currentPosViewPort = mainCamera.WorldToViewportPoint(wallPoint);

            //is visible
            if (0 <= currentPosViewPort.x && currentPosViewPort.x <= 1)
            {
                Vector3 camTargetPosViewPort = mainCamera.WorldToViewportPoint(_cameraInfo.MainCameraTargetPosition);
                Vector3 nearestOrthogonalPoint = GetNearestOrthogonalPoint(wallPoint, mainCamera.transform.position, mainCamera.transform.forward);
                float cameraDistanceToWallPoint = Vector3.Distance(nearestOrthogonalPoint, mainCamera.transform.position);
                float frustumHeight = 2.0f * cameraDistanceToWallPoint * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                float frustumWidth = frustumHeight * mainCamera.aspect;
                Vector3 cameraPosDistanceZ = mainCamera.transform.position + mainCamera.transform.forward * cameraDistanceToWallPoint;
                Vector3 cameraPosHalfWidth = 0.5f * frustumWidth * mainCamera.transform.right;
                Vector3 targetHalfWidth = currentPosViewPort.x <= camTargetPosViewPort.x ? -cameraPosHalfWidth : cameraPosHalfWidth;  
                Vector3 cameraEdgePos = cameraPosDistanceZ + targetHalfWidth;
                Vector3 cameraEdgeDirection = (cameraPosDistanceZ - cameraEdgePos).normalized;
                Vector3 cameraEdgeDiffFromWall = GetNearestOrthogonalPoint(wallPoint, cameraEdgePos, cameraEdgeDirection);
                Vector3 offsetAppliedEdgePos = cameraEdgeDiffFromWall - cameraEdgePos; 
                float normalizedRatio = Mathf.Clamp01((distanceZ + blendDistanceZ - GetTargetDiff().z) / blendDistanceZ);
                return offsetAppliedEdgePos * normalizedRatio;
            }

            return default;
        }
        
        protected virtual bool IsVisible()
        {
            Vector3 viewPort = _cameraInfo.MainCamera.WorldToViewportPoint(transform.position);
            if (0 <= viewPort.x && viewPort.x <= 1)
            {
                float diff = transform.position.z - _cameraInfo.MainCameraTargetPosition.z;
                if (Mathf.Abs(diff) <= distanceZ + blendDistanceZ)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual Vector3 GetTargetDiff()
        {
            return transform.position - _cameraInfo.MainCameraTargetPosition;
        }
        
        protected static Vector3 GetNearestOrthogonalPoint(Vector3 pointX, Vector3 pointA, Vector3 lineDirection)
        {
            Vector3 lineToPointX = pointX - pointA;
            float t = Vector3.Dot(lineDirection, lineToPointX);
            Vector3 nearestPointOnLine = pointA + t * lineDirection;
            return nearestPointOnLine;
        }
    }
}