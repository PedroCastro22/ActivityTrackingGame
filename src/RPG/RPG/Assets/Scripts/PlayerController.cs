using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private GameObject heart1 = null;
    [SerializeField] private GameObject heart2 = null;
    [SerializeField] private GameObject heart3 = null;

    [SerializeField] private Animator playerAnimator = null;

    [SyncVar]
    public string displayName = String.Empty;

    [SyncVar]
    private int health = 3;

    private GameNetworkManager room;

    public GameNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as GameNetworkManager;
        }
    }

    private PlayerSpawnSystem playerSpawn;

    public PlayerSpawnSystem PlayerSpawn
    {
        get
        {
            if (playerSpawn != null) { return playerSpawn; }
            return playerSpawn = PlayerSpawnSystem.Singleton as PlayerSpawnSystem;
        }
    }

    private ScoreSystem scoreSystem;

    public ScoreSystem ScoreSystem
    {
        get
        {
            if (scoreSystem != null) { return scoreSystem; }
            return scoreSystem = ScoreSystem.Singleton as ScoreSystem;
        }
    }

    public override void OnStartAuthority()
    {
        enabled = true;
        Room.Players.Add(this);

        print(Room.GetDifficulty());
    }

    public void TakeHit()
    {
        playerAnimator.SetTrigger("Hit");
        health -= 1;
        if(health == 2)
        {
            heart3.SetActive(false);
        }
        else if(health == 1)
        {
            heart2.SetActive(false);
        }
        else if(health == 0)
        {
            heart1.SetActive(false);
            Die();
        }
    }

    public void Die()
    {
        //NetworkServer.Destroy(gameObject);
        //NetworkServer.DestroyPlayerForConnection(connectionToClient);
        PlayerSpawn.RemovePlayer(gameObject);
        
        if (PlayerSpawn.players.Count == 0)
        {
            print(Room.Players.Count);
            foreach(PlayerController player in Room.Players)
            {
                NetworkServer.Destroy(player.gameObject);
                print("here");
            }
            Room.SetScore(FindObjectOfType<ScoreSystem>().GetScore());
            Room.EndGame();
        }

        GetComponent<PlayerPosition>().enabled = false;
        GetComponent<PlayerRotation>().enabled = false;
        GetComponent<PlayerCameraController>().ChangeTarget();
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

}
