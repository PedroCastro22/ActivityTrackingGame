using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    private void Awake()
    {
        networkManager = FindObjectOfType<GameNetworkManager>();
    }

    public void HostLobby()
    {
        networkManager.StartHost();

        SetAddress();

        print(networkManager.networkAddress);

        landingPagePanel.SetActive(false);
    }

    private void SetAddress()
    {
        string localAddress =  Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(
                    f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();

        networkManager.networkAddress = localAddress;
    }
}
