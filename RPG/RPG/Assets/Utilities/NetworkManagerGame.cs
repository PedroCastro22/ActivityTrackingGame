using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerGame : NetworkManager
{

    GameObject monster;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        SpawnMonster(5);
    }

    public void SpawnMonster(int initialNumber)
    {
        GameObject player = playerPrefab;

        float x = player.transform.position.x + RandomizeSpawn();
        float y = player.transform.position.y + 5;
        float z = player.transform.position.z + RandomizeSpawn();

        for(int i = 0; i <= initialNumber; i++)
        {
            monster = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Monster"), new Vector3(x, y, z), Quaternion.identity);
            NetworkServer.Spawn(monster);

            print(monster.transform.position);

            x = player.transform.position.x + RandomizeSpawn();
            y = player.transform.position.y + 5;
            z = player.transform.position.z + RandomizeSpawn();
        }
    }

    private float RandomizeSpawn()
    {
        float randomPos = Random.Range(5f, 40f);
        bool randomSign = Random.Range(0, 10) < 5;

        if (randomSign)
        {
            return randomPos;
        }
        else
        {
            return randomPos * -1;
        }
    }
}
