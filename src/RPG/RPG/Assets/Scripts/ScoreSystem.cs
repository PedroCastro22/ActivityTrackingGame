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

    public static ScoreSystem Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Singleton = this;
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

    public override void OnStartServer()
    {
        scoreText.text = "0";
    }

    public void SetScore(int value)
    {
        RpcSetScore(value);
    }

    public int GetScore()
    {
        return this.score;
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
        if (score >= 500)
        {
            MaxScore();
        }
    }

    public void MaxScore()
    {
        Room.EndGame();
        Room.SetScore(score);
    }
}
