using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Clean runtime-built leaderboard popup. Replaces the messy editor-baked panel.
public class LeaderboardUI : MonoBehaviour
{
    public static LeaderboardUI Instance;

    private GameObject panel;
    private TMP_Text contentText;

    static readonly Color BG       = new Color(0.05f,  0.025f, 0.01f,  0.94f);
    static readonly Color CARD_BG  = new Color(0.176f, 0.106f, 0.078f, 0.98f);
    static readonly Color GOLD     = new Color(0.973f, 0.773f, 0.404f, 1f);
    static readonly Color GOLD_DIM = new Color(0.973f, 0.773f, 0.404f, 0.22f);
    static readonly Color CREAM    = new Color(0.97f,  0.92f,  0.80f,  1f);
    static readonly Color DARK     = new Color(0.176f, 0.106f, 0.078f, 1f);

    void Awake() { Instance = this; }

    void Start()
    {
        // Remove any leftover editor-baked leaderboard panel
        GameObject stale = FindInactive("LeaderboardPanel");
        if (stale != null && stale != gameObject) Destroy(stale);

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        Build(canvas);
    }

    void Build(Canvas canvas)
    {
        panel = NewImage("LeaderboardUI", canvas.transform, BG);
        RectTransform ort = panel.GetComponent<RectTransform>();
        ort.anchorMin = Vector2.zero; ort.anchorMax = Vector2.one;
        ort.offsetMin = Vector2.zero; ort.offsetMax = Vector2.zero;

        GameObject card = NewImage("Card", panel.transform, CARD_BG);
        RectTransform crt = card.GetComponent<RectTransform>();
        crt.anchorMin = new Vector2(0.5f, 0.5f); crt.anchorMax = new Vector2(0.5f, 0.5f);
        crt.pivot = new Vector2(0.5f, 0.5f); crt.anchoredPosition = Vector2.zero;
        crt.sizeDelta = new Vector2(460f, 430f);

        GameObject bar = NewImage("TopBar", card.transform, GOLD);
        RectTransform brt = bar.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0, 1); brt.anchorMax = new Vector2(1, 1);
        brt.pivot = new Vector2(0.5f, 1f); brt.offsetMin = new Vector2(0, -4f); brt.offsetMax = Vector2.zero;

        Text("TOP SCORES", card.transform, 22f, GOLD, FontStyles.Bold, TextAlignmentOptions.Center,
            new Vector2(0, -20f), new Vector2(400f, 30f), 0f);

        GameObject div = NewImage("Divider", card.transform, GOLD_DIM);
        RectTransform drt = div.GetComponent<RectTransform>();
        drt.anchorMin = new Vector2(0.5f, 1); drt.anchorMax = new Vector2(0.5f, 1);
        drt.pivot = new Vector2(0.5f, 1f); drt.anchoredPosition = new Vector2(0, -56f); drt.sizeDelta = new Vector2(410f, 2f);

        // Column header
        Text("<pos=4%>#<pos=20%>PLAYER<pos=76%>SCORE", card.transform, 12f, GOLD_DIM == GOLD ? GOLD : new Color(0.8f,0.65f,0.4f,1f),
            FontStyles.Bold, TextAlignmentOptions.TopLeft, new Vector2(0, -64f), new Vector2(404f, 20f), 0f);

        GameObject contentGO = Text("", card.transform, 15f, CREAM, FontStyles.Normal, TextAlignmentOptions.TopLeft,
            new Vector2(0, -88f), new Vector2(404f, 270f), 12f);
        contentText = contentGO.GetComponent<TMP_Text>();

        GameObject close = NewButton("BtnCloseLeaderboard", card.transform);
        RectTransform clrt = close.GetComponent<RectTransform>();
        clrt.anchorMin = new Vector2(0.5f, 0); clrt.anchorMax = new Vector2(0.5f, 0);
        clrt.pivot = new Vector2(0.5f, 0); clrt.sizeDelta = new Vector2(160f, 40f); clrt.anchoredPosition = new Vector2(0, 14f);
        GameObject lbl = Text("Close", close.transform, 14f, DARK, FontStyles.Bold, TextAlignmentOptions.Center, Vector2.zero, Vector2.zero, 0f);
        RectTransform lrt = lbl.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one; lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
        close.GetComponent<Button>().onClick.AddListener(Close);

        panel.SetActive(false);
    }

    public void Open()
    {
        if (panel != null) panel.SetActive(true);
        if (contentText != null) contentText.text = "Loading scores...";
        if (ZeroGManager.Instance != null) ZeroGManager.Instance.OnFetchLeaderboardClicked();
    }

    public void Close() { if (panel != null) panel.SetActive(false); }

    public void SetContent(string s) { if (contentText != null) contentText.text = s; }

    // ── Helpers ──

    GameObject FindInactive(string name)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return null;
        foreach (Transform t in canvas.GetComponentsInChildren<Transform>(true))
            if (t.gameObject.name == name) return t.gameObject;
        return null;
    }

    GameObject NewImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.GetComponent<Image>().color = color;
        return go;
    }

    GameObject NewButton(string name, Transform parent)
    {
        GameObject go = NewImage(name, parent, GOLD);
        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1f, 0.941f, 0.82f, 1f);
        cb.pressedColor = new Color(0.827f, 0.612f, 0.345f, 1f);
        cb.colorMultiplier = 1f;
        btn.colors = cb;
        btn.targetGraphic = go.GetComponent<Image>();
        return go;
    }

    GameObject Text(string content, Transform parent, float size, Color color,
        FontStyles style, TextAlignmentOptions align, Vector2 pos, Vector2 sizeDelta, float lineSpacing)
    {
        GameObject go = new GameObject("Txt", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1); rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos; rt.sizeDelta = sizeDelta;
        TMP_Text t = go.GetComponent<TMP_Text>();
        t.text = content; t.fontSize = size; t.fontStyle = style;
        t.color = color; t.alignment = align;
        t.raycastTarget = false; t.enableWordWrapping = false;
        if (lineSpacing != 0f) t.lineSpacing = lineSpacing;
        return go;
    }
}
