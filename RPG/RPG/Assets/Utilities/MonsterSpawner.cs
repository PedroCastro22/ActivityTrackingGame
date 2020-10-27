using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Monster[] monsterTypes;
    [SerializeField] private PlayerController player;
    [SerializeField] private float spawnInterval = 120f;
    [SerializeField] private int initialMonsters = 3;
    [SerializeField] private float minRange = 5f;
    [SerializeField] private float maxRange = 50f;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < initialMonsters; i++)
        {
            SpawnMonster();
        }

        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        while (true)
        {
            SpawnMonster();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnMonster()
    {
        if (player.isLocalPlayer == true)
        {
            int index = Random.Range(0, monsterTypes.Length);
            float x = player.transform.position.x + RandomizeSpawn();
            float y = player.transform.position.y + 5;
            float z = player.transform.position.z + RandomizeSpawn();
            Instantiate(monsterTypes[index], new Vector3(x, y, z), Quaternion.identity);
        }
    }

    private float RandomizeSpawn()
    {
        float randomPos = Random.Range(minRange, maxRange);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
