using UnityEngine;
using UnityEngine.UI;

// Wires ZeroG main-menu buttons at runtime (editor AddListener doesn't serialize).
public class ZeroGUIBinder : MonoBehaviour
{
    void Start()
    {
        // Ensure the clean runtime leaderboard popup exists
        if (FindObjectOfType<LeaderboardUI>() == null)
            new GameObject("LeaderboardUI").AddComponent<LeaderboardUI>();

        Wire("BtnConnectWallet", OnConnect);
        Wire("BtnLeaderboard",   OnLeaderboard);

        // Submitting is now in-game only. Hide the old menu submit button and close the gap.
        GameObject submit = GameObject.Find("BtnSubmitScore");
        if (submit != null) submit.SetActive(false);

        // Move Leaderboard up into the freed slot
        GameObject leader = GameObject.Find("BtnLeaderboard");
        if (leader != null)
        {
            RectTransform lrt = leader.GetComponent<RectTransform>();
            if (lrt != null) lrt.anchoredPosition = new Vector2(lrt.anchoredPosition.x, -88f);
        }

        // Shrink the ZeroG panel to fit two buttons instead of three
        GameObject panel = GameObject.Find("ZeroGPanel");
        if (panel != null)
        {
            RectTransform prt = panel.GetComponent<RectTransform>();
            if (prt != null) prt.sizeDelta = new Vector2(prt.sizeDelta.x, 166f);
        }

        // Tutorial button + How To Play panel owned by HowToPlayPanel.cs
        // Leaderboard popup (open + close) owned by LeaderboardUI.cs
    }

    void Wire(string goName, UnityEngine.Events.UnityAction action)
    {
        GameObject go = GameObject.Find(goName);
        if (go == null) return;
        Button btn = go.GetComponent<Button>();
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }

    void OnConnect() => ZeroGManager.Instance?.OnConnectClicked();

    void OnLeaderboard()
    {
        if (LeaderboardUI.Instance != null) LeaderboardUI.Instance.Open();
    }
}
