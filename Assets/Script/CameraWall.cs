using System;
using System.Linq;
using UnityEngine;

public class CameraWall : MonoBehaviour
{
    public float distanceZ = 1.0f;
    public float blendDistanceZ = 5.0f;
    private bool _isQuitting;

    private void OnEnable()
    {
        CameraManager.Instance.walls.Add(this);
    }

    private void OnDisable()
    {
        if (_isQuitting)
            return;

        CameraManager.Instance.walls.Remove(this);
    }

    public bool IsVisible
    {
        get
        {
            Vector3 viewPort = CameraManager.Instance.mainCamera.WorldToViewportPoint(transform.position);
            if (0 <= viewPort.x && viewPort.x <= 1)
            {
                float diffZ = transform.position.z - CameraManager.Instance.target.transform.position.z;
                if (Mathf.Abs(diffZ) <= distanceZ + blendDistanceZ)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public Vector3 TargetDiff
    {
        get
        {
            return transform.position - CameraManager.Instance.target.transform.position;
        }
    }

    public float DistanceRatio
    {
        get
        {
            var nomralizedRatio = Mathf.Clamp01((distanceZ + blendDistanceZ - TargetDiff.z) / blendDistanceZ);
            return nomralizedRatio * nomralizedRatio * (3f - 2f * nomralizedRatio); //easy in out
        }
    }

    public Vector3 Offset
    {
        get
        {
            Camera mainCamera = CameraManager.Instance.mainCamera;
            GameObject target = CameraManager.Instance.target;
            Vector3 wallPoint = transform.position;
            Vector3 currentPosViewPort = mainCamera.WorldToViewportPoint(wallPoint);
            Vector3 camTargetPosViewPort = mainCamera.WorldToViewportPoint(target.transform.position);
            EdgePoints edgePoints = GetEdgePoints(wallPoint);
            Vector3 wallOffset = default;

            //is visible
            if (0 <= currentPosViewPort.x && currentPosViewPort.x <= 1)
            {
                //left wall
                if (currentPosViewPort.x <= camTargetPosViewPort.x)
                {
                    wallOffset.x = Mathf.Max(-(edgePoints.left.x - wallPoint.x), 0);
                }
                //right walls
                else
                {
                    wallOffset.x = Mathf.Min(-(edgePoints.right.x - wallPoint.x), 0);
                }
            }

            return wallOffset;
        }
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    private static EdgePoints GetEdgePoints(Vector3 wallPoint)
    {
        Camera mainCamera = CameraManager.Instance.mainCamera;
        float distanceZ = wallPoint.z - mainCamera.transform.position.z;
        float frustumHeight = 2.0f * distanceZ * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * mainCamera.aspect;

        Vector3 cameraPosDistanceZ = mainCamera.transform.position + mainCamera.transform.forward * distanceZ;
        Vector3 cameraPosHalfWidth = 0.5f * frustumWidth * mainCamera.transform.right;
        Vector3 leftEdgePos = cameraPosDistanceZ - cameraPosHalfWidth;
        Vector3 rightEdgePos = cameraPosDistanceZ + cameraPosHalfWidth;

        return new EdgePoints()
        {
            left = leftEdgePos,
            right = rightEdgePos
        };
    }

#if UNITY_EDITOR
    /// <summary>
    /// PostProcessVolume.csからコピー
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!UnityEditor.Selection.gameObjects.Contains(gameObject))
            return;

        var center = transform.position;
        var size = new Vector3(1f, 0, distanceZ);
        var blendArea = size + blendDistanceZ * 2f * new Vector3(0, 0, 1);
        var colorRed = new Color(1, 0, 0, 0.4f);
        var colorGreen = new Color(0, 0.5f, 0.5f, 0.2f);
        Gizmos.color = colorRed;
        Gizmos.DrawCube(center, size);
        Gizmos.color = colorGreen;
        Gizmos.DrawCube(center, blendArea);
        Gizmos.color = Color.black;
        //Gizmos.DrawWireCube(center, size);
        //Gizmos.DrawWireCube(center, blendArea);
    }
#endif
}

public struct EdgePoints
{
    public Vector3 left;
    public Vector3 right;
}
