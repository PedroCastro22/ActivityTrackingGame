using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : NetworkBehaviour
{
    private Rigidbody rb; 

    [SerializeField] private float spawnRate = 0.1f;
    [SerializeField] private float health = 100;
    [SerializeField] private float attack = 0;
    [SerializeField] private float defense = 0;
    [SerializeField] private Text text;

    private int xp = 0;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

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
        NetworkClient.connection.identity.GetComponent<PlayerController>().CmdTakePrize(gameObject);
    }

    [ClientRpc]
    private void HandleMovement()
    {
        float xDiff = transform.position.x - NetworkClient.connection.identity.GetComponent<PlayerController>().CmdGetX();
        float yDiff = transform.position.y - NetworkClient.connection.identity.GetComponent<PlayerController>().CmdGetY();
        if (Mathf.Abs(xDiff) < 10f && Mathf.Abs(yDiff) < 10f)
        {
            transform.LookAt(NetworkClient.connection.identity.GetComponent<PlayerController>().transform);
            transform.Translate(0.0f, 0.0f, 7f * Time.deltaTime);
        }
        
    }

    [Server]
    private void Update()
    {
        HandleMovement();
    }
}
