using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using System.Linq;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private GameObject inputPagePanel = null;
    [SerializeField] private GameObject creditsPagePanel = null;
    [SerializeField] private GameObject instructionsPagePanel = null;
    [SerializeField] private TMP_Text gameTitle = null;

    private void Awake()
    {
        networkManager = FindObjectOfType<GameNetworkManager>();
    }

    public void HostLobby()
    {
        gameTitle.gameObject.SetActive(false);

        networkManager.StartHost();

        SetAddress();

        print(networkManager.networkAddress);

        landingPagePanel.SetActive(false);
    }

    public void Credits()
    {
        creditsPagePanel.SetActive(true);
    }

    public void CreditsBack()
    {
        creditsPagePanel.SetActive(false);
    }

    public void Instructions()
    {
        instructionsPagePanel.SetActive(true);
    }

    public void InstructionsBack()
    {
        instructionsPagePanel.SetActive(false);
    }

    private void SetAddress()
    {
        string localAddress =  Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(
                    f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();

        networkManager.networkAddress = localAddress;
    }

    public void GoBackToInput()
    {
        inputPagePanel.SetActive(true);

        landingPagePanel.SetActive(false);
    }
}
