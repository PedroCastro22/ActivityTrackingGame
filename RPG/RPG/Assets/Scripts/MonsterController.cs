using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MonsterController : NetworkBehaviour
{
    private PlayerSpawnSystem playerSpawn;

    public PlayerSpawnSystem PlayerSpawn
    {
        get
        {
            if (playerSpawn != null) { return playerSpawn; }
            return playerSpawn = PlayerSpawnSystem.Singleton as PlayerSpawnSystem;
        }
    }

    private GameNetworkManager room;

    public GameNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as GameNetworkManager;
        }
    }

    [SyncVar]
    private int health = 3;

    private float moveSpeed = 1f;

    private int target = 0;

    private List<GameObject> players = null;

    public override void OnStartServer()
    {
        enabled = true;

        SetTarget(GetRandomPlayerNumber());

        SetMoveSpeed();

        players = PlayerSpawn.players;

        print("Monster spawned at " + transform.position);
    }

    private Vector3 GetPlayerPosition(int num)
    {
        return PlayerSpawn.players[num].transform.position;
    }

    private int GetRandomPlayerNumber()
    {
        int rand = Random.Range(0, PlayerSpawn.players.Count);

        return rand;
    }

    private void SetTarget(int target)
    {
        this.target = target;
    }

    private int GetTarget()
    {
        return this.target;
    }

    private void SetMoveSpeed()
    {
        moveSpeed = Room.GetDifficulty() + 1f;
    }

    private void OnMouseDown() => RpcClickedOn();

    [Command(ignoreAuthority = true)]
    private void RpcClickedOn()
    {
        if (health > 1)
        {
            health -= 1;
        }
        else if (health == 1)
        {
            NetworkServer.Destroy(gameObject);
            FindObjectOfType<ScoreSystem>().SetScore(10);
        }
    }

    [ServerCallback]
    private void Update() => Move();

    [Server]
    private void Move()
    {
        Vector3 pos = GetPlayerPosition(GetTarget());

        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * moveSpeed);

        for (int i = 0; i < PlayerSpawn.players.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, PlayerSpawn.players[i].transform.position);
            if (dist < 0.5f)
            {
                PlayerSpawn.players[i].GetComponent<PlayerController>().TakeHit();
                NetworkServer.Destroy(gameObject);
            }
        }

    }
}
