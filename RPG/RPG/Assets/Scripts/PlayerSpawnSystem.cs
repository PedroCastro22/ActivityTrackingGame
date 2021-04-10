using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;

    public List<GameObject> players = new List<GameObject>();

    public static PlayerSpawnSystem Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    public override void OnStartServer() => GameNetworkManager.OnServerReadied += SpawnPlayer;

    [ServerCallback]
    private void OnDestroy() => GameNetworkManager.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        GameObject playerInstance = Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        NetworkServer.Spawn(playerInstance, conn);

        players.Add(playerInstance);

        print("Player Count: " + players.Count);
    }

    public void RemovePlayer(GameObject player)
    {
        players.Remove(player);
        print("Player Count: " + players.Count);
    }
}
