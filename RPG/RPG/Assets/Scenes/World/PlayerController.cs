using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
   
    [SerializeField] private int xpRequired;
    [SerializeField] private int levelBase = 100;

    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject Sphere;
    [SerializeField] private GameObject Cube;
    [SerializeField] private Text xpText;

    [SyncVar(hook = nameof(AddXp))]
    private int xp = 0;

    private void Awake()
    {
        this.gameObject.SetActive(true);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Sphere.GetComponent<MeshRenderer>().material.color = Color.blue;
        Cube.GetComponent<MeshRenderer>().material.color = Color.blue;
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

    private void AddXp(System.Int32 oldValue, System.Int32 newValue)
    {
        print(newValue);
        xpText.text = newValue.ToString();
    }

    [Command]
    public void CmdTakePrize(GameObject prize)
    {
        NetworkServer.Destroy(prize);
        xp += 20;
    }

    public float CmdGetX()
    {
        return gameObject.transform.position.x;
    }

    public float CmdGetY()
    {
        return gameObject.transform.position.y;
    }

}
