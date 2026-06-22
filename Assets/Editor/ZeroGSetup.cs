using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class ZeroGSetup
{
    // Palette konsisten dengan MiningSystem
    static readonly Color PANEL_BG      = new Color(0.176f, 0.106f, 0.078f, 0.96f);
    static readonly Color PANEL_OVERLAY = new Color(0.176f, 0.106f, 0.078f, 0.88f);
    static readonly Color BTN_GOLD      = new Color(0.973f, 0.773f, 0.404f, 1f);
    static readonly Color BTN_HOVER     = new Color(1f,     0.941f, 0.82f,  1f);
    static readonly Color BTN_PRESSED   = new Color(0.827f, 0.612f, 0.345f, 1f);
    static readonly Color BTN_MUTED     = new Color(0.55f,  0.38f,  0.22f,  1f);
    static readonly Color TEXT_CREAM    = new Color(1f,     0.949f, 0.741f, 1f);
    static readonly Color TEXT_DARK     = new Color(0.176f, 0.106f, 0.078f, 1f);
    static readonly Color TEXT_DIM      = new Color(0.9f,   0.78f,  0.55f,  1f);

    // ─────────────────────────────────────────────
    [MenuItem("0G/Setup ZeroG UI")]
    static void Setup()
    {
        Canvas canvas = FindOrCreateCanvas();

        Transform old = canvas.transform.Find("ZeroGPanel");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // Panel pojok kanan atas
        GameObject panel = CreatePanel("ZeroGPanel", canvas.transform,
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(-12, -12), new Vector2(300, 210), new Vector2(1, 1));

        GameObject statusGO = CreateText("TxtWalletStatus", panel.transform,
            "Wallet: not connected", 12f, TEXT_DIM, FontStyles.Italic,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(12, -12), new Vector2(276, 24), new Vector2(0, 1));

        GameObject btnConnect = CreateButton("BtnConnectWallet", panel.transform, "Connect OKX Wallet",
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -44), new Vector2(276, 38));

        GameObject btnSubmit = CreateButton("BtnSubmitScore", panel.transform, "Submit Score to 0G",
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -88), new Vector2(276, 38));
        btnSubmit.GetComponent<Button>().interactable = false;
        btnSubmit.GetComponent<Image>().color = BTN_MUTED;

        GameObject btnLeader = CreateButton("BtnLeaderboard", panel.transform, "Leaderboard",
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -132), new Vector2(276, 38));

        // Leaderboard Panel — tengah layar
        GameObject leaderPanel = CreatePanel("LeaderboardPanel", canvas.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(420, 320), new Vector2(0.5f, 0.5f));
        leaderPanel.SetActive(false);

        GameObject leaderTitle = CreateText("TxtLeaderTitle", leaderPanel.transform,
            "TOP SCORES", 20f, TEXT_CREAM, FontStyles.Bold,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -16), new Vector2(380, 30), new Vector2(0.5f, 1));

        GameObject leaderText = CreateText("TxtLeaderboard", leaderPanel.transform,
            "", 13f, TEXT_DIM, FontStyles.Normal,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -56), new Vector2(380, 220), new Vector2(0.5f, 1));
        leaderText.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.TopLeft;

        GameObject btnClose = CreateButton("BtnCloseLeaderboard", leaderPanel.transform, "Close",
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 14), new Vector2(160, 36));
        btnClose.GetComponent<Image>().color = BTN_MUTED;

        ZeroGManager mgr = Object.FindObjectOfType<ZeroGManager>();
        if (mgr == null)
        {
            GameObject mgrGO = new GameObject("ZeroGManager");
            mgr = mgrGO.AddComponent<ZeroGManager>();
        }

        mgr.walletStatusText       = statusGO.GetComponent<TMP_Text>();
        mgr.connectWalletButton    = btnConnect.GetComponent<Button>();
        mgr.submitScoreButton      = btnSubmit.GetComponent<Button>();
        mgr.fetchLeaderboardButton = btnLeader.GetComponent<Button>();
        mgr.leaderboardPanel       = leaderPanel;
        mgr.leaderboardText        = leaderText.GetComponent<TMP_Text>();

        // ZeroGUIBinder wires buttons at runtime (AddListener doesn't serialize)
        ZeroGUIBinder binder = mgr.gameObject.GetComponent<ZeroGUIBinder>();
        if (binder == null) binder = mgr.gameObject.AddComponent<ZeroGUIBinder>();

        EditorUtility.SetDirty(mgr);
        EditorUtility.SetDirty(binder);
        Debug.Log("[0G Setup] ZeroG UI done!");
    }

    // ─────────────────────────────────────────────
    [MenuItem("0G/Setup AI UI")]
    static void SetupAIUI()
    {
        Canvas canvas = FindOrCreateCanvas();

        AIService aiSvc = Object.FindObjectOfType<AIService>();
        if (aiSvc == null)
            aiSvc = new GameObject("AIService").AddComponent<AIService>();

        // Lore Panel — bawah tengah
        Transform oldLore = canvas.transform.Find("LorePanel");
        if (oldLore != null) Object.DestroyImmediate(oldLore.gameObject);

        GameObject lorePanel = CreatePanel("LorePanel", canvas.transform,
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 16), new Vector2(520, 100), new Vector2(0.5f, 0));
        lorePanel.GetComponent<Image>().color = PANEL_BG;

        // Garis tipis atas sebagai dekorasi
        GameObject accent = new GameObject("Accent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        accent.transform.SetParent(lorePanel.transform, false);
        accent.transform.localRotation = Quaternion.identity;
        accent.transform.localScale = Vector3.one;
        RectTransform accentRT = accent.GetComponent<RectTransform>();
        accentRT.anchorMin = new Vector2(0, 1); accentRT.anchorMax = new Vector2(1, 1);
        accentRT.pivot = new Vector2(0.5f, 1);
        accentRT.offsetMin = new Vector2(0, -3); accentRT.offsetMax = Vector2.zero;
        accent.GetComponent<Image>().color = BTN_GOLD;

        GameObject loadingGO = CreateText("TxtLoading", lorePanel.transform,
            "The dog is sniffing the artifact...", 13f, TEXT_DIM, FontStyles.Italic,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(490, 80), new Vector2(0.5f, 0.5f));

        GameObject loreTextGO = CreateText("TxtLore", lorePanel.transform,
            "", 13f, TEXT_CREAM, FontStyles.Normal,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(490, 80), new Vector2(0.5f, 0.5f));

        ArtifactLoreUI loreUI = lorePanel.AddComponent<ArtifactLoreUI>();
        loreUI.lorePanel   = lorePanel;
        loreUI.loreText    = loreTextGO.GetComponent<TMP_Text>();
        loreUI.loadingText = loadingGO.GetComponent<TMP_Text>();
        lorePanel.SetActive(false);

        // Speech Bubble — pojok kiri bawah, gaya game
        Transform oldBubble = canvas.transform.Find("SpeechBubble");
        if (oldBubble != null) Object.DestroyImmediate(oldBubble.gameObject);

        GameObject bubble = CreatePanel("SpeechBubble", canvas.transform,
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(16, 130), new Vector2(260, 72), new Vector2(0, 0));
        bubble.GetComponent<Image>().color = PANEL_BG;

        // Aksen emas kiri
        GameObject sideAccent = new GameObject("SideAccent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        sideAccent.transform.SetParent(bubble.transform, false);
        sideAccent.transform.localRotation = Quaternion.identity;
        sideAccent.transform.localScale = Vector3.one;
        RectTransform saRT = sideAccent.GetComponent<RectTransform>();
        saRT.anchorMin = new Vector2(0, 0); saRT.anchorMax = new Vector2(0, 1);
        saRT.pivot = new Vector2(0, 0.5f);
        saRT.offsetMin = Vector2.zero; saRT.offsetMax = new Vector2(4, 0);
        sideAccent.GetComponent<Image>().color = BTN_GOLD;

        GameObject bubbleText = CreateText("TxtDog", bubble.transform,
            "", 12f, TEXT_CREAM, FontStyles.Normal,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(4, 0), new Vector2(240, 60), new Vector2(0.5f, 0.5f));

        DogCompanion dog = aiSvc.gameObject.GetComponent<DogCompanion>();
        if (dog == null) dog = aiSvc.gameObject.AddComponent<DogCompanion>();
        dog.speechBubble = bubble;
        dog.speechText   = bubbleText.GetComponent<TMP_Text>();
        bubble.SetActive(false);

        EditorUtility.SetDirty(aiSvc);
        EditorUtility.SetDirty(dog);
        EditorUtility.SetDirty(loreUI);
        Debug.Log("[0G Setup] AI UI done!");
    }

    // ─────────────────────────────────────────────
    [MenuItem("0G/Setup GameOver UI")]
    static void SetupGameOver()
    {
        Canvas canvas = FindOrCreateCanvas();

        Transform old = canvas.transform.Find("GameOverPanel");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // Overlay gelap
        GameObject panel = new GameObject("GameOverPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        panel.transform.localRotation = Quaternion.identity;
        panel.transform.localScale = Vector3.one;
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero; panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero; panelRT.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.08f, 0.04f, 0.02f, 0.93f);

        // Card tengah
        GameObject card = CreatePanel("Card", panel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(340, 260), new Vector2(0.5f, 0.5f));

        // Garis emas atas card
        GameObject topBar = new GameObject("TopBar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        topBar.transform.SetParent(card.transform, false);
        topBar.transform.localRotation = Quaternion.identity;
        topBar.transform.localScale = Vector3.one;
        RectTransform tbRT = topBar.GetComponent<RectTransform>();
        tbRT.anchorMin = new Vector2(0, 1); tbRT.anchorMax = new Vector2(1, 1);
        tbRT.pivot = new Vector2(0.5f, 1);
        tbRT.offsetMin = new Vector2(0, -5); tbRT.offsetMax = Vector2.zero;
        topBar.GetComponent<Image>().color = BTN_GOLD;

        GameObject title = CreateText("TxtGameOver", card.transform,
            "GAME OVER", 42f, BTN_GOLD, FontStyles.Bold,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -24), new Vector2(300, 54), new Vector2(0.5f, 1));

        GameObject scoreText = CreateText("TxtGameOverScore", card.transform,
            "Score: 0 pts", 18f, TEXT_CREAM, FontStyles.Normal,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -86), new Vector2(300, 30), new Vector2(0.5f, 1));

        GameObject btnRestart = CreateButton("BtnRestart", card.transform, "Try Again",
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -130), new Vector2(220, 44));

        GameObject btnMenu = CreateButton("BtnMainMenu", card.transform, "Back to Menu",
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -184), new Vector2(220, 44));
        btnMenu.GetComponent<Image>().color = BTN_MUTED;

        GameOverUI goUI = panel.AddComponent<GameOverUI>();
        goUI.restartButton  = btnRestart.GetComponent<Button>();
        goUI.mainMenuButton = btnMenu.GetComponent<Button>();

        SatisfyingMiner miner = Object.FindObjectOfType<SatisfyingMiner>();
        if (miner != null)
        {
            miner.gameOverPanel     = panel;
            miner.gameOverScoreText = scoreText.GetComponent<TMP_Text>();
            EditorUtility.SetDirty(miner);
        }

        panel.SetActive(false);
        EditorUtility.SetDirty(goUI);
        Debug.Log("[0G Setup] Game Over UI done!");
    }

    // ─────────────────────────────────────────────
    [MenuItem("0G/Setup How To Play UI")]
    static void SetupHowToPlay()
    {
        Canvas canvas = FindOrCreateCanvas();

        // Remove old panel only — don't touch existing button
        Transform old = canvas.transform.Find("HowToPlayPanel");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // Full-screen dimmer
        GameObject overlay = new GameObject("HowToPlayPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        overlay.transform.SetParent(canvas.transform, false);
        overlay.transform.localRotation = Quaternion.identity;
        overlay.transform.localScale = Vector3.one;
        RectTransform overlayRT = overlay.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero; overlayRT.anchorMax = Vector2.one;
        overlayRT.offsetMin = Vector2.zero; overlayRT.offsetMax = Vector2.zero;
        overlay.GetComponent<Image>().color = new Color(0.06f, 0.03f, 0.01f, 0.91f);

        // Card
        GameObject card = CreatePanel("Card", overlay.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero,
            new Vector2(560f, 500f), new Vector2(0.5f, 0.5f));

        // Gold top bar
        GameObject topBar = new GameObject("TopBar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        topBar.transform.SetParent(card.transform, false);
        topBar.transform.localRotation = Quaternion.identity;
        topBar.transform.localScale = Vector3.one;
        RectTransform tbRT = topBar.GetComponent<RectTransform>();
        tbRT.anchorMin = new Vector2(0, 1); tbRT.anchorMax = new Vector2(1, 1);
        tbRT.pivot = new Vector2(0.5f, 1); tbRT.offsetMin = new Vector2(0, -5); tbRT.offsetMax = Vector2.zero;
        topBar.GetComponent<Image>().color = BTN_GOLD;

        // Title
        CreateText("TxtHTPTitle", card.transform, "HOW TO PLAY", 26f, BTN_GOLD, FontStyles.Bold,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -20f), new Vector2(520f, 36f), new Vector2(0.5f, 1));

        // Divider
        GameObject div = new GameObject("Divider", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        div.transform.SetParent(card.transform, false);
        div.transform.localRotation = Quaternion.identity;
        div.transform.localScale = Vector3.one;
        RectTransform divRT = div.GetComponent<RectTransform>();
        divRT.anchorMin = new Vector2(0.5f, 1); divRT.anchorMax = new Vector2(0.5f, 1);
        divRT.pivot = new Vector2(0.5f, 1); divRT.anchoredPosition = new Vector2(0, -60f); divRT.sizeDelta = new Vector2(500f, 1f);
        div.GetComponent<Image>().color = new Color(0.973f, 0.773f, 0.404f, 0.3f);

        string content =
            "<color=#F8C566><b>CONTROLS</b></color>\n" +
            "  <color=#F0E4BD>A</color>  move / dig left      " +
            "<color=#F0E4BD>D</color>  move / dig right\n" +
            "  <color=#F0E4BD>S</color>  dig down             " +
            "<color=#F0E4BD>B</color>  open artifact guide\n\n" +
            "<color=#F8C566><b>DIGGING</b></color>\n" +
            "  Hold a key to keep digging. Deeper than depth 20 costs 2 energy per block.\n\n" +
            "<color=#F8C566><b>LOOT</b></color>\n" +
            "  <color=#F8C566>Gold</color>  +5 pts     <color=#A8DFFF>Diamond</color>  +20 pts     <color=#7DEFA8>Energy tile</color>  +energy\n" +
            "  Artifacts give +30 to +100 pts — rarer ones hide deeper.\n\n" +
            "<color=#F8C566><b>ENERGY</b></color>\n" +
            "  Watch the energy bar — hit zero and you can't dig anymore.\n" +
            "  Surface only: spend 50 pts at the Shop to refill energy.\n\n" +
            "<color=#F8C566><b>SURFACING</b></color>\n" +
            "  Use Back to Top when energy's empty or you're deep enough.\n" +
            "  Your score auto-saves to the 0G blockchain when you surface.\n\n" +
            "<color=#F8C566><b>DOG</b></color>\n" +
            "  Your dog companion drops AI-powered tips as you play. Listen up!";

        GameObject bodyText = CreateText("TxtHTPBody", card.transform, content,
            13f, TEXT_CREAM, FontStyles.Normal,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -74f), new Vector2(510f, 360f), new Vector2(0.5f, 1));
        var tmp = bodyText.GetComponent<TMP_Text>();
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.lineSpacing = 4f;

        // Close button
        GameObject btnClose = CreateButton("BtnCloseHTP", card.transform, "Got it!",
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 16f), new Vector2(180f, 40f));

        overlay.SetActive(false);
        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("[0G Setup] How To Play UI done!");
    }

    // ─────────────────────────────────────────────
    // Helpers

    static Canvas FindOrCreateCanvas()
    {
        Canvas c = Object.FindObjectOfType<Canvas>();
        if (c != null) return c;
        GameObject go = new GameObject("Canvas");
        c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        go.AddComponent<GraphicRaycaster>();
        return c;
    }

    static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, Vector2 pivot)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot = pivot; rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.GetComponent<Image>().color = PANEL_BG;
        return go;
    }

    static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 1); rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.GetComponent<Image>().color = BTN_GOLD;

        Button btn = go.GetComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor      = Color.white;
        cb.highlightedColor = BTN_HOVER;
        cb.pressedColor     = BTN_PRESSED;
        cb.selectedColor    = BTN_HOVER;
        cb.colorMultiplier  = 1f;
        cb.fadeDuration     = 0.1f;
        btn.colors = cb;
        btn.targetGraphic = go.GetComponent<Image>();

        CreateText("Label", go.transform, label, 14f, TEXT_DARK, FontStyles.Bold,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f), stretch: true);

        return go;
    }

    static GameObject CreateText(string name, Transform parent, string content, float fontSize,
        Color color, FontStyles style, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pos, Vector2 size, Vector2 pivot, bool stretch = false)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot = pivot; rt.anchoredPosition = pos;
        if (stretch) { rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero; }
        else rt.sizeDelta = size;

        TMP_Text txt = go.GetComponent<TMP_Text>();
        txt.text = content; txt.fontSize = fontSize;
        txt.fontStyle = style; txt.color = color;
        txt.alignment = TextAlignmentOptions.Center;
        txt.enableWordWrapping = true;
        txt.raycastTarget = false;
        return go;
    }
}
