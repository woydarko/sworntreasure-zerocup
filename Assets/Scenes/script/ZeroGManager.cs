using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class ZeroGManager : MonoBehaviour
{
    public static ZeroGManager Instance;

    [Header("Wallet UI — assign in main-menu scene")]
    public TMP_Text walletStatusText;
    public Button connectWalletButton;
    public Button submitScoreButton;
    public Button fetchLeaderboardButton;
    public GameObject leaderboardPanel;
    public TMP_Text leaderboardText;

    public string WalletAddress { get; private set; } = "";

    // Fired whenever the wallet/submit status changes, so in-game UI can react.
    public System.Action<string> StatusChanged;

    // Hashes disimpan di memory, bukan PlayerPrefs — semua player share via 0G
    // Untuk demo: tetap simpan lokal sebagai fallback
    private List<string> storedHashes = new List<string>();

    [DllImport("__Internal")]
    private static extern void ConnectWallet();
    [DllImport("__Internal")]
    private static extern void SubmitScore(string address, string score);
    [DllImport("__Internal")]
    private static extern void FetchLeaderboard(string hashesJson);

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        storedHashes = LoadHashes();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Re-bind UI refs setiap kali scene load
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        walletStatusText       = FindInScene<TMP_Text>(scene, "TxtWalletStatus");
        connectWalletButton    = FindInScene<Button>(scene, "BtnConnectWallet");
        submitScoreButton      = FindInScene<Button>(scene, "BtnSubmitScore");
        fetchLeaderboardButton = FindInScene<Button>(scene, "BtnLeaderboard");
        leaderboardPanel       = FindInScene<GameObject>(scene, "LeaderboardPanel");
        leaderboardText        = FindInScene<TMP_Text>(scene, "TxtLeaderboard");

        // Re-apply state setelah scene load
        if (!string.IsNullOrEmpty(WalletAddress))
        {
            string shortAddr = WalletAddress.Length > 10
                ? WalletAddress.Substring(0, 6) + "..." + WalletAddress.Substring(WalletAddress.Length - 4)
                : WalletAddress;
            UpdateStatus("Connected: " + shortAddr);
            if (submitScoreButton != null) submitScoreButton.interactable = true;

            MainMenu menu = FindObjectOfType<MainMenu>();
            if (menu != null) menu.OnWalletReady();
        }
        else
        {
            if (submitScoreButton != null) submitScoreButton.interactable = false;
        }
    }

    // Searches the loaded scene including INACTIVE objects (GameObject.Find skips inactive,
    // which broke leaderboard refs that live inside an inactive panel).
    T FindInScene<T>(Scene scene, string goName) where T : Object
    {
        if (!scene.IsValid()) return null;
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.gameObject.name != goName) continue;
                if (typeof(T) == typeof(GameObject)) return t.gameObject as T;
                return t.GetComponent<T>();
            }
        }
        return null;
    }

    // ── Button handlers ──

    public void OnConnectClicked()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ConnectWallet();
#else
        OnWalletConnected("0xDEMO1234EDITOR5678");
#endif
    }

    public void OnSubmitScoreClicked()
    {
        if (string.IsNullOrEmpty(WalletAddress)) { UpdateStatus("Connect your wallet first!"); return; }
        var miner = FindObjectOfType<SatisfyingMiner>();
        if (miner == null) return;
        UpdateStatus("Submitting to 0G...");
#if UNITY_WEBGL && !UNITY_EDITOR
        SubmitScore(WalletAddress, miner.currentPoints.ToString());
#else
        OnScoreSubmitted("DEMO_HASH_" + miner.currentPoints);
#endif
    }

    public void OnFetchLeaderboardClicked()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FetchLeaderboard("");
#else
        OnLeaderboardData("[{\"address\":\"0xDEMO1234EDITOR5678\",\"score\":999,\"timestamp\":0},{\"address\":\"0xABCD5678EFGH9012\",\"score\":500,\"timestamp\":0}]");
#endif
    }

    // ── JS Callbacks ──

    public void OnWalletConnected(string address)
    {
        WalletAddress = address;
        string shortAddr = address.Length > 10
            ? address.Substring(0, 6) + "..." + address.Substring(address.Length - 4)
            : address;
        UpdateStatus("Connected: " + shortAddr);
        if (submitScoreButton != null) submitScoreButton.interactable = true;

        MainMenu menu = FindObjectOfType<MainMenu>();
        if (menu != null) menu.OnWalletReady();
    }

    public void OnWalletError(string message)
    {
        UpdateStatus("Wallet error: " + message);
    }

    public void OnScoreSubmitted(string txHash)
    {
        if (!string.IsNullOrEmpty(txHash) && !storedHashes.Contains(txHash))
        {
            storedHashes.Add(txHash);
            SaveHashes();
        }
        UpdateStatus("Score on-chain! ✓");
        Debug.Log("[ZeroG] Tx: " + txHash);
    }

    public void OnSubmitError(string message)
    {
        UpdateStatus("Submit failed: " + message);
    }

    public void OnLeaderboardData(string json)
    {
        var entries = JsonUtility.FromJson<LeaderboardWrapper>("{\"entries\":" + json + "}");

        if (entries == null || entries.entries == null || entries.entries.Length == 0)
        {
            if (LeaderboardUI.Instance != null) LeaderboardUI.Instance.SetContent("No scores yet.\nBe the first!");
            else if (leaderboardText != null) leaderboardText.text = "No scores yet. Be the first!";
            return;
        }

        // Columns aligned with TMP <pos> tags so they stay neat regardless of address length
        string display = "";
        for (int i = 0; i < entries.entries.Length; i++)
        {
            var e = entries.entries[i];
            string shortAddr = e.address.Length > 10
                ? e.address.Substring(0, 6) + "..." + e.address.Substring(e.address.Length - 4)
                : e.address;
            display += $"<pos=4%>{i + 1}.<pos=20%>{shortAddr}<pos=76%>{e.score}\n";
        }
        display = display.TrimEnd();

        if (LeaderboardUI.Instance != null) LeaderboardUI.Instance.SetContent(display);
        else if (leaderboardText != null) leaderboardText.text = display;
    }

    // ── Helpers ──

    void UpdateStatus(string msg)
    {
        if (walletStatusText != null) walletStatusText.text = msg;
        StatusChanged?.Invoke(msg);
        Debug.Log("[ZeroG] " + msg);
    }

    List<string> LoadHashes()
    {
        string raw = PlayerPrefs.GetString("ZeroG_Hashes", "");
        if (string.IsNullOrEmpty(raw)) return new List<string>();
        var list = new List<string>(raw.Split(','));
        list.RemoveAll(string.IsNullOrEmpty);
        return list;
    }

    void SaveHashes()
    {
        PlayerPrefs.SetString("ZeroG_Hashes", string.Join(",", storedHashes));
        PlayerPrefs.Save();
    }

    [System.Serializable] class LeaderboardEntry { public string address; public int score; public long timestamp; }
    [System.Serializable] class LeaderboardWrapper { public LeaderboardEntry[] entries; }
}
