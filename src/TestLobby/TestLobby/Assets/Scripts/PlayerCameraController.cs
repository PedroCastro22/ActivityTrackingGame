using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    public override void OnStartAuthority()
    {

        virtualCamera.gameObject.SetActive(true);

        enabled = true;

        virtualCamera.m_Lens.OrthographicSize = 5f;

    }
}
