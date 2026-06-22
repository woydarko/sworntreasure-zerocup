using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Single source of truth for the How To Play panel.
// Finds the existing "Tutorial" button, wires it, and builds the panel at runtime.
// Cleans up any leftover scene-baked panel so there is never a duplicate.
public class HowToPlayPanel : MonoBehaviour
{
    private GameObject panel;

    static readonly Color BG       = new Color(0.05f,  0.025f, 0.01f,  0.94f);
    static readonly Color CARD_BG  = new Color(0.176f, 0.106f, 0.078f, 0.98f);
    static readonly Color GOLD     = new Color(0.973f, 0.773f, 0.404f, 1f);
    static readonly Color GOLD_DIM = new Color(0.973f, 0.773f, 0.404f, 0.22f);
    static readonly Color CREAM    = new Color(0.97f,  0.92f,  0.80f,  1f);
    static readonly Color DARK     = new Color(0.176f, 0.106f, 0.078f, 1f);

    void Start()
    {
        // Remove any leftover panel baked into the scene by the old editor script
        GameObject stale = FindInactive("HowToPlayPanel");
        if (stale != null && stale != gameObject) Destroy(stale);

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        BuildPanel(canvas);

        GameObject tutBtn = GameObject.Find("Tutorial");
        if (tutBtn != null)
        {
            Button btn = tutBtn.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(Show);
            }
        }
    }

    void BuildPanel(Canvas canvas)
    {
        // Fullscreen dimmer
        panel = NewImage("HowToPlayOverlay", canvas.transform, BG);
        RectTransform overlayRT = panel.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero; overlayRT.anchorMax = Vector2.one;
        overlayRT.offsetMin = Vector2.zero; overlayRT.offsetMax = Vector2.zero;

        // Card
        GameObject card = NewImage("Card", panel.transform, CARD_BG);
        RectTransform cardRT = card.GetComponent<RectTransform>();
        cardRT.anchorMin = new Vector2(0.5f, 0.5f); cardRT.anchorMax = new Vector2(0.5f, 0.5f);
        cardRT.pivot = new Vector2(0.5f, 0.5f); cardRT.anchoredPosition = Vector2.zero;
        cardRT.sizeDelta = new Vector2(540f, 540f);

        // Gold top bar
        GameObject bar = NewImage("TopBar", card.transform, GOLD);
        RectTransform barRT = bar.GetComponent<RectTransform>();
        barRT.anchorMin = new Vector2(0, 1); barRT.anchorMax = new Vector2(1, 1);
        barRT.pivot = new Vector2(0.5f, 1f);
        barRT.offsetMin = new Vector2(0, -4f); barRT.offsetMax = Vector2.zero;

        // Title
        Text("HOW TO PLAY", card.transform, 23f, GOLD, FontStyles.Bold, TextAlignmentOptions.Center,
            new Vector2(0, -22f), new Vector2(480f, 32f), 0f);

        // Divider under title
        GameObject div = NewImage("Divider", card.transform, GOLD_DIM);
        RectTransform divRT = div.GetComponent<RectTransform>();
        divRT.anchorMin = new Vector2(0.5f, 1); divRT.anchorMax = new Vector2(0.5f, 1);
        divRT.pivot = new Vector2(0.5f, 1f);
        divRT.anchoredPosition = new Vector2(0, -60f); divRT.sizeDelta = new Vector2(476f, 2f);

        string body =
            "<color=#F8C566><b>CONTROLS</b></color>\n" +
            "Press <color=#F0E4BD>A</color> or <color=#F0E4BD>D</color> to dig left and right, and <color=#F0E4BD>S</color> to dig straight down. Press <color=#F0E4BD>B</color> anytime to open your artifact guide.\n\n" +
            "<color=#F8C566><b>DIGGING</b></color>\n" +
            "Hold a key to keep digging. Every block costs a bit of energy, and once you pass depth 20 it costs double, so pace yourself down there.\n\n" +
            "<color=#F8C566><b>WHAT YOU'LL FIND</b></color>\n" +
            "Gold gives you <color=#F8C566>+5 points</color>. Artifacts are the real prize at <color=#D4A0FF>+30 to +100</color>, with rarer ones buried deeper. Energy tiles refill your energy, so grab them.\n\n" +
            "<color=#F8C566><b>RUNNING LOW</b></color>\n" +
            "Hit zero energy and you can't dig. Head to the surface and refill at the Shop for 50 points.\n\n" +
            "<color=#F8C566><b>HEADING HOME</b></color>\n" +
            "Use Back to Top once you're empty or deep enough to climb back up.\n\n" +
            "<color=#F8C566><b>SAVING YOUR SCORE</b></color>\n" +
            "Tap <color=#F8C566>Submit Score</color> to lock your points on-chain. It's the only way to save. Die before submitting and your points are gone, so don't get too greedy.\n\n" +
            "<color=#F8C566><b>YOUR DOG</b></color>\n" +
            "Your dog tags along and drops little AI tips while you play. Worth listening to!";

        Text(body, card.transform, 11.5f, CREAM, FontStyles.Normal, TextAlignmentOptions.TopLeft,
            new Vector2(0, -66f), new Vector2(476f, 420f), 1.5f);

        // Close button
        GameObject closeBtn = NewButton("BtnCloseHTP", card.transform);
        RectTransform cbRT = closeBtn.GetComponent<RectTransform>();
        cbRT.anchorMin = new Vector2(0.5f, 0); cbRT.anchorMax = new Vector2(0.5f, 0);
        cbRT.pivot = new Vector2(0.5f, 0); cbRT.sizeDelta = new Vector2(180f, 40f);
        cbRT.anchoredPosition = new Vector2(0, 16f);
        GameObject lbl = Text("Got it!", closeBtn.transform, 14f, DARK, FontStyles.Bold, TextAlignmentOptions.Center, Vector2.zero, Vector2.zero, 0f);
        RectTransform lblRT = lbl.GetComponent<RectTransform>();
        lblRT.anchorMin = Vector2.zero; lblRT.anchorMax = Vector2.one;
        lblRT.offsetMin = Vector2.zero; lblRT.offsetMax = Vector2.zero;
        closeBtn.GetComponent<Button>().onClick.AddListener(Hide);

        panel.SetActive(false);
    }

    public void Show() { if (panel != null) panel.SetActive(true); }
    public void Hide() { if (panel != null) panel.SetActive(false); }

    // ── Helpers ──────────────────────────────────────────

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
        t.enableWordWrapping = true; t.raycastTarget = false;
        if (lineSpacing != 0f) t.lineSpacing = lineSpacing;
        return go;
    }
}
