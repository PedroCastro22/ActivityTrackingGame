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

    private Transform targetPlayer = null;

    [SerializeField] private Animator enemyAnimator = null;

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

        targetPlayer = PlayerSpawn.players[target].GetComponent<PlayerController>().transform;
    }

    private int GetTarget()
    {
        return this.target;
    }

    private void SetMoveSpeed()
    {
        moveSpeed = (Room.GetDifficulty() * .3f) + .5f;
    }

    private void OnMouseDown() => RpcClickedOn();

    [Command(ignoreAuthority = true)]
    private void RpcClickedOn()
    {
        if (health > 0)
        {
            if(health == 1)
            {
                DieAfterAnimation();
                FindObjectOfType<ScoreSystem>().SetScore(10);
                health -= 1;
                return;
            }
            health -= 1;
        }
        else { return; }
    }

    private void Die()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void DieAfterAnimation()
    {
        enemyAnimator.SetTrigger("Die");

        print("here");

        Invoke(nameof(Die), 3);
    }

    [ServerCallback]
    private void Update() => Move();

    [Server]
    private void Move()
    {
        if (health > 0)
        {
            Vector3 pos = GetPlayerPosition(GetTarget());

            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * moveSpeed);

            transform.LookAt(targetPlayer);

            for (int i = 0; i < PlayerSpawn.players.Count; i++)
            {
                float dist = Vector3.Distance(transform.position, PlayerSpawn.players[i].transform.position);
                if (dist < 2.5f)
                {
                    PlayerSpawn.players[i].GetComponent<PlayerController>().TakeHit();
                    NetworkServer.Destroy(gameObject);
                }
            }
        }
    }
}
