using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MonsterSpawnerSystem : NetworkBehaviour
{
    [SerializeField] private GameObject monster = null;

    private Vector3 GetRandomPlayerPosition()
    {
        int rand = Random.Range(0, PlayerSpawnSystem.players.Count);

        Vector3 playerPos = PlayerSpawnSystem.players[rand].transform.position;

        Vector3 spawnPos = new Vector3
        {
            x = playerPos.x - 2,
            y = playerPos.y - 2,
            z = playerPos.z
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
        if(PlayerSpawnSystem.players.Count == 0)
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
