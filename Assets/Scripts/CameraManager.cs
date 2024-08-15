using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _idleCamera;
    [SerializeField] private CinemachineVirtualCamera _followCamera;

    private void Awake() {
        SwitchToIdleCamera();
    }

    public void SwitchToIdleCamera() {
        _idleCamera.enabled = true;
        _followCamera.enabled = false;
    }

    public void SwitchToFollowCamera(Transform followTransform) {
        _followCamera.Follow = followTransform;

        _followCamera.enabled = true;
        _idleCamera.enabled = false;
    }
}
