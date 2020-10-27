//using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text levelText;
    [SerializeField] private GameObject menu;

    public void UpdateLevel(int level)
    {
        levelText.text = level.ToString();
    }

    public void ToggleMenu()
    {
        menu.SetActive(true);
    }

    public void UntoggleMenu()
    {
        menu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
}
