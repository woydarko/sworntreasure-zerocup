using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Button restartButton;
    public Button mainMenuButton;

    void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestart);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    void OnRestart()
    {
        SatisfyingMiner miner = FindObjectOfType<SatisfyingMiner>();
        if (miner != null) miner.RestartGame();
    }

    void OnMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
