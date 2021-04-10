using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NetworkRoomPlayer : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private GameObject difficultyUI = null;
    [SerializeField] private Text[] playerNameTexts = new Text[4];
    [SerializeField] private Text[] playerReadyTexts = new Text[4];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button changeDifficultyButton = null;
    [SerializeField] private Button readyButton = null;
    [SerializeField] private Text ipText = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading ...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;

    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
            changeDifficultyButton.gameObject.SetActive(value);
            readyButton.gameObject.SetActive(!value);
        }
    }

    private GameNetworkManager room;

    public GameNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as GameNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        UpdateDisplay();

        SetIP();
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.isLocalPlayer)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player ...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartGame();
    }

    public void HandleDifficulty()
    {
        bool active = difficultyUI.activeSelf;

        difficultyUI.SetActive(!active);
    }

    [Command]
    public void CmdSetEasy()
    {
        Room.SetDifficulty(0);
        IsReady = true;
        difficultyUI.SetActive(false);
    }

    [Command]
    public void CmdSetMedium()
    {
        Room.SetDifficulty(1);
        IsReady = true;
        difficultyUI.SetActive(false);
    }

    [Command]
    public void CmdSetHard()
    {
        Room.SetDifficulty(2);
        IsReady = true;
        difficultyUI.SetActive(false);
    }

    public void SetIP()
    {
        ipText.text = Room.networkAddress;
    }
}
