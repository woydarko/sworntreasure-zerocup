using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DogCompanion : MonoBehaviour
{
    public static DogCompanion Instance;

    public GameObject speechBubble;
    public TMP_Text speechText;
    public float commentDuration = 4f;
    public float cooldownSeconds = 15f;

    private float lastCommentTime = -99f;
    private Coroutine hideCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-bind UI refs in the new scene by name
        GameObject bubble = GameObject.Find("SpeechBubble");
        if (bubble != null)
        {
            speechBubble = bubble;
            TMP_Text txt = bubble.GetComponentInChildren<TMP_Text>(includeInactive: true);
            if (txt != null) speechText = txt;
        }
    }

    bool CanComment() => Time.time - lastCommentTime >= cooldownSeconds;

    public void TriggerComment(string context)
    {
        if (!CanComment()) return;
        lastCommentTime = Time.time;

        if (AIService.Instance != null)
            AIService.Instance.RequestDogComment(context);
        else
            ShowComment("Woof! Let's keep digging!");
    }

    public void ShowComment(string comment)
    {
        if (speechBubble == null || speechText == null) return;
        speechText.text = comment;
        speechBubble.SetActive(true);

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(commentDuration);
        if (speechBubble != null) speechBubble.SetActive(false);
    }

    public void OnFirstDig() => TriggerComment("player just started digging underground for the first time");
    public void OnDeepReached(int depth) => TriggerComment($"player reached depth {depth}, it's getting really deep and dark underground");
    public void OnLowEnergy(int energy) => TriggerComment($"player only has {energy} energy left, almost exhausted");
    public void OnArtifactFound(string name) => TriggerComment($"artifact found: {name}");
    public void OnGameOver() => TriggerComment("game over, player ran out of energy and points");
    public void OnBackToSurface() => TriggerComment("player made it back to the surface safely");
}
