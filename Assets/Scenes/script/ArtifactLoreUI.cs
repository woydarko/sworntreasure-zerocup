using UnityEngine;
using TMPro;

public class ArtifactLoreUI : MonoBehaviour
{
    public static ArtifactLoreUI Instance;

    public GameObject lorePanel;
    public TMP_Text loreText;
    public TMP_Text loadingText;
    public float autoDismissSeconds = 6f;

    private Coroutine dismissCoroutine;

    // Per-scene singleton. Lives in-game only, recreated each load.
    // No DontDestroyOnLoad — this is a child of Canvas; persisting it would detach it.
    void Awake()
    {
        Instance = this;
    }

    public void ShowPending(string artifactName)
    {
        if (lorePanel == null) return;
        if (loadingText != null) loadingText.gameObject.SetActive(true);
        if (loreText != null) loreText.gameObject.SetActive(false);
        if (loadingText != null) loadingText.text = $"Reading the {artifactName}...";
        lorePanel.SetActive(true);
    }

    public void ShowLore(string lore)
    {
        if (lorePanel == null) return;
        if (loadingText != null) loadingText.gameObject.SetActive(false);
        if (loreText != null)
        {
            loreText.gameObject.SetActive(true);
            loreText.text = lore;
        }
        lorePanel.SetActive(true);

        if (dismissCoroutine != null) StopCoroutine(dismissCoroutine);
        dismissCoroutine = StartCoroutine(AutoDismiss());
    }

    public void Dismiss()
    {
        if (dismissCoroutine != null) StopCoroutine(dismissCoroutine);
        if (lorePanel != null) lorePanel.SetActive(false);
    }

    System.Collections.IEnumerator AutoDismiss()
    {
        yield return new WaitForSeconds(autoDismissSeconds);
        Dismiss();
    }
}
