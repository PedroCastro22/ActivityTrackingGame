using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private int xp;
    [SerializeField] private int xpRequired;
    [SerializeField] private int levelBase = 100;

    [SerializeField] private GameObject playerCamera;

    private int level = 1;

    private void Awake()
    {
        this.gameObject.SetActive(true);
    }

    void Start()
    {
        if(isLocalPlayer == true)
        {
            playerCamera.SetActive(true);
        }
        else
        {
            playerCamera.SetActive(false);
        }
    }

    public int GetXp()
    {
        return this.xp;
    }

    public int GetXpRequired()
    {
        return this.xpRequired;
    }

    public int GetLevel()
    {
        return this.levelBase;
    }

    public void AddXp(int newXp)
    {
        this.xp += newXp;
    }

}
