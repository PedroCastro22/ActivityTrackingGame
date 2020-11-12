using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : NetworkBehaviour
{
    [SerializeField] private float spawnRate = 0.1f;
    [SerializeField] private float health = 100;
    [SerializeField] private float attack = 0;
    [SerializeField] private float defense = 0;

    public float GetSpawnRate()
    {
        return this.spawnRate;
    }

    public float GetHealth()
    {
        return this.health;
    }

    public float GetAttack()
    {
        return this.attack;
    }

    public float GetDefense()
    {
        return this.defense;
    }

    private void OnMouseDown()
    {
        
    }

}
