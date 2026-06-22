using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject creditsPanel;

    [Header("0G")]
    public Button playButton;

    void Start()
    {
#if UNITY_EDITOR
        if (playButton != null) playButton.interactable = true;
#else
        if (playButton != null) playButton.interactable = false;
#endif
    }

    // Dipanggil ZeroGManager setelah wallet connect berhasil
    public void OnWalletReady()
    {
        if (playButton != null)
            playButton.interactable = true;
    }

    public void PlayGame()
    {
#if !UNITY_EDITOR
        if (ZeroGManager.Instance == null || string.IsNullOrEmpty(ZeroGManager.Instance.WalletAddress))
        {
            Debug.Log("Connect your wallet first!");
            return;
        }
#endif
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Fungsi untuk membuka Credits
    public void OpenCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
    }

    // Fungsi untuk menutup Credits
    public void CloseCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}