using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Cinemachine;

public class PlayerController : NetworkBehaviour
{
    private GameNetworkManager room;

    public GameNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as GameNetworkManager;
        }
    }

    private CharacterController controller;

    private float moveSpeed = 5f;

    [SyncVar]
    public string displayName = String.Empty;

    [SyncVar]
    private int health = 5;

    private Vector2 previousInput;

    private Controls controls;

    private Controls Controls
    {
        get
        {
            if(controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        Room.Players.Add(this);

        print("Player Count: " + Room.Players.Count);
    }

    public override void OnStopClient()
    {
        Room.Players.Remove(this);
    }

    public override void OnStartAuthority()
    {
        enabled = true;

        controller = GetComponent<CharacterController>();

        controller.detectCollisions = false;

        Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.Player.Move.canceled += ctx => ResetMovement();
    }

    [ClientCallback]
    private void OnEnable()
    {
        Controls.Enable();
    }

    [ClientCallback]
    private void OnDisable() => Controls.Disable();

    [ClientCallback]
    private void Update() => Move();

    [Client]
    private void SetMovement(Vector2 movement) => previousInput = movement;

    [Client]
    private void ResetMovement() => previousInput = Vector2.zero;

    [Client]
    private void Move()
    {
        if (IsAlive())
        {
            Vector3 right = controller.transform.right;
            Vector3 up = controller.transform.up;
            right.z = 0f;
            up.z = 0f;

            Vector3 movement = right.normalized * previousInput.x + up.normalized * previousInput.y;
            controller.Move(movement * Time.deltaTime * moveSpeed);
        }

        else
        {
            Die();
        }
        
    }

    private bool IsAlive()
    {
        if(health > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TakeHit()
    {
        health -= 1;
        print(health);
    }

    public void Die()
    {
        NetworkServer.Destroy(gameObject);
        NetworkServer.DestroyPlayerForConnection(connectionToClient);

        if(Room.Players.Count == 0)
        {
            Room.EndGame();
        }
        print("Player Count: " + Room.Players.Count);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

}
