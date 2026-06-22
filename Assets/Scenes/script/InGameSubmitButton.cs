using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Builds the in-game "Submit Score to 0G" button at runtime.
// This is the ONLY way to record a score on-chain. If the player dies
// without submitting, the score is lost — that is the intended risk.
public class InGameSubmitButton : MonoBehaviour
{
    static readonly Color BTN_GOLD  = new Color(0.973f, 0.773f, 0.404f, 1f);
    static readonly Color BTN_HOVER = new Color(1f,     0.941f, 0.82f,  1f);
    static readonly Color BTN_PRESS = new Color(0.827f, 0.612f, 0.345f, 1f);
    static readonly Color TEXT_DARK = new Color(0.176f, 0.106f, 0.078f, 1f);
    static readonly Color TEXT_DIM  = new Color(0.9f,   0.78f,  0.55f,  1f);

    private TMP_Text statusText;

    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Button — bottom-right
        GameObject btnGO = new GameObject("BtnSubmitScoreInGame",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnGO.transform.SetParent(canvas.transform, false);
        btnGO.transform.localRotation = Quaternion.identity;
        btnGO.transform.localScale = Vector3.one;
        RectTransform brt = btnGO.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(1, 0); brt.anchorMax = new Vector2(1, 0);
        brt.pivot = new Vector2(1, 0);
        brt.anchoredPosition = new Vector2(-16f, 16f);
        brt.sizeDelta = new Vector2(220f, 46f);
        btnGO.GetComponent<Image>().color = BTN_GOLD;

        Button btn = btnGO.GetComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = BTN_HOVER;
        cb.pressedColor = BTN_PRESS;
        cb.colorMultiplier = 1f;
        btn.colors = cb;
        btn.targetGraphic = btnGO.GetComponent<Image>();
        btn.onClick.AddListener(OnSubmitClicked);

        MakeText("Label", btnGO.transform, "Submit Score", 15f, TEXT_DARK,
            FontStyles.Bold, TextAlignmentOptions.Center, true, Vector2.zero, Vector2.zero);

        // Status text above the button
        GameObject stGO = MakeText("InGameSubmitStatus", canvas.transform, "", 12f, TEXT_DIM,
            FontStyles.Italic, TextAlignmentOptions.Right, false,
            new Vector2(-16f, 70f), new Vector2(300f, 22f));
        RectTransform srt = stGO.GetComponent<RectTransform>();
        srt.anchorMin = new Vector2(1, 0); srt.anchorMax = new Vector2(1, 0);
        srt.pivot = new Vector2(1, 0);
        statusText = stGO.GetComponent<TMP_Text>();

        // Subscribe to status updates from ZeroGManager
        if (ZeroGManager.Instance != null)
            ZeroGManager.Instance.StatusChanged += OnStatus;
    }

    void OnDestroy()
    {
        if (ZeroGManager.Instance != null)
            ZeroGManager.Instance.StatusChanged -= OnStatus;
    }

    void OnSubmitClicked()
    {
        if (ZeroGManager.Instance == null)
        {
            if (statusText != null) statusText.text = "Wallet not ready";
            return;
        }
        ZeroGManager.Instance.OnSubmitScoreClicked();
    }

    void OnStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }

    GameObject MakeText(string name, Transform parent, string content, float size, Color color,
        FontStyles style, TextAlignmentOptions align, bool stretch, Vector2 pos, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        RectTransform rt = go.GetComponent<RectTransform>();
        if (stretch)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }
        else
        {
            rt.anchoredPosition = pos; rt.sizeDelta = sizeDelta;
        }
        TMP_Text t = go.GetComponent<TMP_Text>();
        t.text = content; t.fontSize = size; t.fontStyle = style;
        t.color = color; t.alignment = align;
        t.raycastTarget = false; t.enableWordWrapping = true;
        return go;
    }
}
