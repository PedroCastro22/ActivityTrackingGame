using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MonsterSpawnerSystem : NetworkBehaviour
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

    [SerializeField] private GameObject monster = null;

    private Vector3 GetRandomPlayerPosition()
    {
        int rand = Random.Range(0, PlayerSpawn.players.Count);

        Vector3 playerPos = PlayerSpawn.players[rand].transform.position;

        Vector3 spawnPos = new Vector3
        {
            x = playerPos.x - 6,
            y = playerPos.y,
            z = playerPos.z - 6
        };

        return spawnPos;
    }

    public override void OnStartServer()
    {
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        while (true)
        {
            SpawnMonster();
            yield return new WaitForSeconds(5);
        }
    }

    private void SpawnMonster()
    {
        if (PlayerSpawn.players.Count == 0)
        {
            GameObject monsterInstance = Instantiate(monster);
            NetworkServer.Spawn(monsterInstance);
        }
        else
        {
            Vector3 pos = GetRandomPlayerPosition();
            GameObject monsterInstance = Instantiate(monster, pos, Quaternion.identity);
            NetworkServer.Spawn(monsterInstance);
        }
    }
}
