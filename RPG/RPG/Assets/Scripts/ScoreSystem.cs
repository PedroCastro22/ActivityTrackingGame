using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class ScoreSystem : NetworkBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    [SyncVar(hook = nameof(SetScoreText))]
    private int score = 0;

    private GameNetworkManager room;

    public GameNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as GameNetworkManager;
        }
    }

    public override void OnStartServer()
    {
        scoreText.text = "0";
    }

    public void SetScore(int value)
    {
        RpcSetScore(value);
    }

    private void SetScoreText(int oldValue, int newValue)
    {
        scoreText.text = newValue.ToString();
    }

    [ClientRpc]
    public void RpcSetScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
        //if (score >= 30)
        //{
        //    MaxScore();
        //}
    }

    public void MaxScore()
    {
        NetworkServer.Destroy(gameObject);
        Room.EndGame();
    }
}
