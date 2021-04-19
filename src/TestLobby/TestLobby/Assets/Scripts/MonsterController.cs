using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MonsterController : NetworkBehaviour
{
    [SyncVar]
    private int health = 3;

    private float moveSpeed = 3f;

    private int target = 0;

    private List<GameObject> players = null; 

    public override void OnStartServer()
    {
        enabled = true;

        SetTarget(GetRandomPlayerNumber());

        players = PlayerSpawnSystem.players;

        print("Monster spawned at " + transform.position);
    }

    private Vector3 GetPlayerPosition(int num)
    {
        return PlayerSpawnSystem.players[num].transform.position;
    }

    private int GetRandomPlayerNumber()
    {
        int rand = Random.Range(0, PlayerSpawnSystem.players.Count);

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

    private void OnMouseDown() => RpcClickedOn();

    [Command(ignoreAuthority = true)]
    private void RpcClickedOn()
    {
        if(health > 1)
        {
            health -= 1;
        }
        else if(health == 1)
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

        transform.position = Vector2.MoveTowards(transform.position, pos, moveSpeed * Time.deltaTime);

        for(int i = 0; i < players.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, players[i].transform.position);
            if (dist < 1f)
            {
                players[i].GetComponent<PlayerController>().TakeHit();
                NetworkServer.Destroy(gameObject);
            }
        }
        
    }
}
