using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonMonoBehaviour<CameraManager>
{
    public Camera mainCamera;

    public Vector3 offset;

    public GameObject target;

    public bool useCameraWall = true;

    public List<CameraWall> walls;
    
    private void Update()
    {
        mainCamera.transform.position = target.transform.position + offset;

        if (useCameraWall && TryGetClosestWall(out var cameraWall))
        {
            Vector3 cameraWallOffset = cameraWall.Offset * cameraWall.DistanceRatio;
            mainCamera.transform.Translate(cameraWallOffset, Space.World);
        }
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 30;
        GUI.Label(new Rect(0, 0, 400, 100), $"Use CameraWall: { useCameraWall }");
    }

    private bool TryGetClosestWall(out CameraWall closestWall)
    {
        closestWall = null;

        foreach (CameraWall wall in walls)
        {
            if (wall.IsVisible == false)
            {
                continue;
            }

            if (closestWall == null || closestWall.TargetDiff.sqrMagnitude > wall.TargetDiff.sqrMagnitude)
            {
                closestWall = wall;
            }
        }

        return closestWall != null;
    }
}