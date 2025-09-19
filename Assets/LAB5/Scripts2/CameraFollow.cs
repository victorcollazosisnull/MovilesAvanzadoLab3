using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private CinemachineCamera _camera;

    private void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();
    }

    private void Update()
    {
        if (_camera.Target.TrackingTarget != null) return;

        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.SpawnManager != null &&
            NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject() != null)
        {
            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;
            _camera.Target.TrackingTarget = player;
        }
    }
}