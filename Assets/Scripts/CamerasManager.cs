using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


public enum CameraType
{
    TPC, 
    Aim,
}

public class CamerasManager : MonoBehaviour
{
    [Serializable]
    class CameraData
    {
        public CameraType cameraType;
        public CinemachineCamera camera;
    } 

    [SerializeField]
    private List<CameraData> m_cameras;

    private void Start()
    {
        ChangeCamera(CameraType.TPC);
    }

    public void ChangeCamera(CameraType cameraType)
    {
        foreach (var item in m_cameras)
        {
            item.camera.gameObject.SetActive(item.cameraType == cameraType);
        }
    }

    public void SetTarget(Transform target)
    {
        foreach (var item in m_cameras)
        {
            item.camera.Follow = target;
        }
    }
}
