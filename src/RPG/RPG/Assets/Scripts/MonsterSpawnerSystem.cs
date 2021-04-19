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

        var list = new[] { 6, 7, 8, 9, -6, -7, -8, -9};
        var random = new System.Random();

        Vector3 playerPos = PlayerSpawn.players[rand].transform.position;

        Vector3 spawnPos = new Vector3
        {
            x = playerPos.x - list[random.Next(list.Length)],
            y = playerPos.y,
            z = playerPos.z - list[random.Next(list.Length)]
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
            yield return new WaitForSeconds(2);
        }
    }

    private void SpawnMonster()
    {
        if (PlayerSpawn.players.Count == 0)
        {
            //GameObject monsterInstance = Instantiate(monster);
            //NetworkServer.Spawn(monsterInstance);
        }
        else
        {
            Vector3 pos = GetRandomPlayerPosition();
            GameObject monsterInstance = Instantiate(monster, pos, Quaternion.identity);
            NetworkServer.Spawn(monsterInstance);
        }
    }
}
