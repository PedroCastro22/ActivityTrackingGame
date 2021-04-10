using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{

    public static Player localPlayer;
    [SyncVar] public string matchID;

    [SyncVar] public int playerIndex;

    NetworkMatchChecker networkMatchChecker;

    private void Start()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
        if (isLocalPlayer)
        {
            localPlayer = this;
        }
        else
        {
            UILobby.instance.SpawnPlayerUIPrefab(this);
        }
    }

    // Host game

    public void HostGame()
    {
        string matchID = Matchmaker.GetRandomMatchID();
        CmdHostGame(matchID);
    }

    [Command]
    void CmdHostGame(string _matchID)
    {
        matchID = _matchID;
        if (Matchmaker.instance.HostGame(_matchID, gameObject, out playerIndex))
        {
            Debug.Log($"<color=green>Game hosted successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid();
            TargetHostGame(true, _matchID, playerIndex);
        }
        else
        {
            Debug.Log($"<color=red>Game host failed</color>");
            TargetHostGame(false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetHostGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"Match ID: {matchID} == {_matchID}");
        UILobby.instance.HostSuccess(success, _matchID);
    }

    // Join Game

    public void JoinGame(string _inputID)
    {
        CmdJoinGame(_inputID);
    }

    [Command]
    void CmdJoinGame(string _matchID)
    {
        matchID = _matchID;
        if (Matchmaker.instance.JoinGame(_matchID, gameObject, out playerIndex))
        {
            Debug.Log($"<color=green>Game hosted successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid();
            TargetJoinGame(true, _matchID, playerIndex);
        }
        else
        {
            Debug.Log($"<color=red>Game host failed</color>");
            TargetJoinGame(false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetJoinGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"Match ID: {matchID} == {_matchID}");
        UILobby.instance.JoinSuccess(success, _matchID);
    }

    // Begin game

    public void BeginGame()
    {
        CmdBeginGame();
    }

    [Command]
    void CmdBeginGame()
    {
        Matchmaker.instance.BeginGame(matchID);
        Debug.Log($"<color=green>Game beggining</color>");
    }

    public void StartGame()
    {
        TargetBeginGame();
    }

    [TargetRpc]
    void TargetBeginGame()
    {

        Scene currentScene = SceneManager.GetActiveScene();

        Debug.Log($"Match ID: {matchID} | Beggining");

        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }
}
