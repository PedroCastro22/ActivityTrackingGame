using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobby : MonoBehaviour
{

    public static UILobby instance;

    [Header("Host Join")]
    [SerializeField] InputField ipAddressInput;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;
    [SerializeField] Canvas lobbyCanvas;

    [Header("Lobby")]
    [SerializeField] Transform PlayerUIParent;
    [SerializeField] GameObject PlayerUIPrefab;
    [SerializeField] Text matchIDText;
    [SerializeField] GameObject beginButton;

    private void Start()
    {
        instance = this;
    }

    public void Host()
    {
        ipAddressInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        Player.localPlayer.HostGame();
    }

    public void HostSuccess(bool success, string matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;

            SpawnPlayerUIPrefab(Player.localPlayer);
            matchIDText.text = matchID;
            beginButton.SetActive(true);
        }
        else
        {
            ipAddressInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void Join()
    {
        ipAddressInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        Player.localPlayer.JoinGame(ipAddressInput.text.ToUpper());
    }

    public void JoinSuccess(bool success, string matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;

            SpawnPlayerUIPrefab(Player.localPlayer);
            matchIDText.text = matchID;
        }
        else
        {
            ipAddressInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void BeginGame()
    {
        Player.localPlayer.BeginGame();
    }

    public void SpawnPlayerUIPrefab(Player player)
    {
        GameObject newPlayerUI = Instantiate(PlayerUIPrefab, PlayerUIParent);
        newPlayerUI.GetComponent<PlayerUI>().SetPlayer(player);
        newPlayerUI.transform.SetSiblingIndex(player.playerIndex - 1);
    }
}
