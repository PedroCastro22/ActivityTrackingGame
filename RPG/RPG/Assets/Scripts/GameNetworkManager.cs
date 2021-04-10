using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameNetworkManager : NetworkManager
{
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private GameObject monsterSpawnSystem = null;
    [SerializeField] private GameObject locationBasedSystem = null;
    [SerializeField] private GameObject scoreSystem = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    private int difficulty = 0;

    public List<NetworkRoomPlayer> RoomPlayers { get; } = new List<NetworkRoomPlayer>();
    public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();
    public List<PlayerController> Players { get; } = new List<PlayerController>();

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        var spawnnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers > maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayer>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }
        base.OnServerDisconnect(conn);
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }

    private bool IsReadyToStart()
    {
        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }

    public void StartGame()
    {
        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene)
        {
            if (!IsReadyToStart())
            {
                return;
            }

            ServerChangeScene("GameScene");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene && newSceneName.StartsWith("Game"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                gamePlayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("GameScene"))
        {
            GameObject locationBasedSystemInstance = Instantiate(locationBasedSystem);
            NetworkServer.Spawn(locationBasedSystemInstance);

            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);

            GameObject monsterSpawnSystemInstance = Instantiate(monsterSpawnSystem);
            NetworkServer.Spawn(monsterSpawnSystemInstance);

            GameObject scoreSystemInstance = Instantiate(scoreSystem);
            NetworkServer.Spawn(scoreSystemInstance);
        }
        if (sceneName.StartsWith("End"))
        {
            NetworkServer.Shutdown();
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    public void EndGame()
    {
        ServerChangeScene("EndScene");
    }

    // Variables
    public void SetDifficulty(int value)
    {
        this.difficulty = value;
        print(difficulty);
    }

    public int GetDifficulty()
    {
        return this.difficulty;
    }
}