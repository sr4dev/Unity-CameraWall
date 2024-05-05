using System.Collections.Generic;
using UnityEngine;

namespace PerspectiveCameraWall.Core
{
    public class CameraWall : MonoBehaviour
    {
        private enum OffsetType
        {
            None,
            Negative,
            Positive
        }
        
        private static List<CameraWall> _walls = new(100);
        private static ICameraInfo _cameraInfo;

        public float distanceZ;
        public float blendDistanceZ;

        #region Unity Event
        /// <summary>
        /// Support for Enter Play Mode
        /// https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RunOnStart()
        {
            _walls.Clear();
            _cameraInfo = default;
        }

        private void OnEnable()
        {
            _walls.Add(this);
        }

        private void OnDisable()
        {
            _walls.Remove(this);
        }
        #endregion
        
        public static void SetInfo<T>(T cameraInfo) where T : ICameraInfo, new()
        {
            _cameraInfo = cameraInfo;
        }
        
        private OffsetType TryCalculateOffset(out Vector3 offset)
        {
            Camera mainCamera = _cameraInfo.MainCamera;
            Vector3 wallPoint = transform.position;
            Vector3 viewportWall = mainCamera.WorldToViewportPoint(wallPoint);

            //is visible
            if (0 <= viewportWall.x && viewportWall.x <= 1 && 0 <= viewportWall.y && viewportWall.y <= 1)
            {
                Vector3 viewportCamTarget = mainCamera.WorldToViewportPoint(_cameraInfo.MainCameraTargetPosition);
                Vector3 nearestOrthogonalPoint = GetNearestOrthogonalPoint(wallPoint, mainCamera.transform.position, mainCamera.transform.forward);
                float cameraDistanceToWallPoint = Vector3.Distance(nearestOrthogonalPoint, mainCamera.transform.position);
                float frustumHeight = 2.0f * cameraDistanceToWallPoint * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                float frustumWidth = frustumHeight * mainCamera.aspect;
                Vector3 cameraPosDistanceZ = mainCamera.transform.position + mainCamera.transform.forward * cameraDistanceToWallPoint;

                // Vector3 cameraPosHalfHeight = 0.5f * frustumHeight * mainCamera.transform.up;
                // Vector3 targetHalfHeight = viewportWall.y <= viewportCamTarget.y ? cameraPosHalfHeight : -cameraPosHalfHeight;//up, down
                Vector3 cameraPosHalfWidth = 0.5f * frustumWidth * mainCamera.transform.right;
                bool isNegativeSide = viewportWall.x <= viewportCamTarget.x;
                Vector3 targetHalfWidth = isNegativeSide ? -cameraPosHalfWidth : cameraPosHalfWidth;//left, right

                Vector3 cameraTargetAndCameraOrthogonalPoint = GetNearestOrthogonalPoint(_cameraInfo.MainCameraTargetPosition, mainCamera.transform.position, mainCamera.transform.forward);
                Vector3 cameraTargetAndCameraOrthogonalDirection = (cameraTargetAndCameraOrthogonalPoint - mainCamera.transform.position).normalized; 

                Vector3 wallAndCameraTargetOrthogonalPoint = GetNearestOrthogonalPoint(wallPoint, _cameraInfo.MainCameraTargetPosition, cameraTargetAndCameraOrthogonalDirection);
                float wallAndCameraTargetTopDownViewDistance = Vector3.Distance(wallAndCameraTargetOrthogonalPoint, _cameraInfo.MainCameraTargetPosition);
                float normalizedRatio = Mathf.Clamp01((distanceZ + blendDistanceZ - wallAndCameraTargetTopDownViewDistance) / blendDistanceZ);
                
                Vector3 cameraEdgePos = cameraPosDistanceZ + targetHalfWidth;
                Vector3 cameraEdgeDirection = (cameraPosDistanceZ - cameraEdgePos).normalized;
                Vector3 cameraEdgeDiffFromWall = GetNearestOrthogonalPoint(wallPoint, cameraEdgePos, cameraEdgeDirection);
                Vector3 offsetAppliedEdgePos = cameraEdgeDiffFromWall - cameraEdgePos;
                offset = offsetAppliedEdgePos * normalizedRatio;
                return isNegativeSide ? OffsetType.Negative : OffsetType.Positive;
            }

            offset = default;
            return OffsetType.None;
        }

        private static Vector3 GetNearestOrthogonalPoint(Vector3 pointX, Vector3 pointA, Vector3 pointADirection)
        {
            Vector3 lineToPointX = pointX - pointA;
            float t = Vector3.Dot(pointADirection, lineToPointX);
            Vector3 nearestPointOnLine = pointA + t * pointADirection;
            return nearestPointOnLine;
        }

        public static Vector3 GetTotalOffset()
        {
            Vector3 offsetNegativeSide = default;
            Vector3 offsetPositiveSide = default;

            foreach (var wall in _walls)
            {
                switch (wall.TryCalculateOffset(out var offset))
                {
                    case OffsetType.None:
                        break;
                    
                    case OffsetType.Negative:
                        if (offsetNegativeSide.sqrMagnitude < offset.sqrMagnitude)
                        {
                            offsetNegativeSide = offset;
                        }
                        break;
                    
                    case OffsetType.Positive:
                        if (offsetPositiveSide.sqrMagnitude < offset.sqrMagnitude)
                        {
                            offsetPositiveSide = offset;
                        }
                        break;
                }
            }

            return offsetNegativeSide + offsetPositiveSide;
        }
    }
}