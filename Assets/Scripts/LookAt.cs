using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        Vector3 dir = transform.position - mainCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
