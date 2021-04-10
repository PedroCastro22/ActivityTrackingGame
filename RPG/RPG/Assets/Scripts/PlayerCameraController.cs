using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    private PlayerSpawnSystem playerSpawn;

    public PlayerSpawnSystem PlayerSpawn
    {
        get
        {
            if (playerSpawn != null) { return playerSpawn; }
            return playerSpawn = PlayerSpawnSystem.Singleton as PlayerSpawnSystem;
        }
    }

    public override void OnStartAuthority()
    {
        virtualCamera.gameObject.SetActive(true);

        enabled = true;
    }

    public void ChangeTarget()
    {
        if(PlayerSpawn.players.Count > 0)
        {
            int rand = Random.Range(0, PlayerSpawn.players.Count);

            Transform newTarget = PlayerSpawn.players[rand].transform;

            virtualCamera.m_LookAt = newTarget;
            virtualCamera.m_Follow = newTarget;
        }
    }
}
