using UnityEngine;
using System.Runtime.InteropServices;

public class AIService : MonoBehaviour
{
    public static AIService Instance;

    [Header("OpenRouter Config")]
    public string apiKey = "YOUR_OPENROUTER_API_KEY";
    public string model = "meta-llama/llama-3.1-8b-instruct";

    [DllImport("__Internal")]
    private static extern void InitAI(string apiKey, string model);

    [DllImport("__Internal")]
    private static extern void CallAI(string callbackName, string prompt);

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        InitAI(apiKey, model);
#endif
    }

    public void RequestArtifactLore(string artifactName, string rarity, string description)
    {
        string prompt = $"You're a quirky archaeologist AI assistant in a game. A player just found a {rarity} artifact: \"{artifactName}\". Original description: {description}. Write 2 short casual sentences of fun lore about this artifact. Keep it exciting and casual, max 30 words total.";
#if UNITY_WEBGL && !UNITY_EDITOR
        CallAI("OnArtifactLoreReady", prompt);
#else
        OnArtifactLoreReady($"Whoa, the {artifactName}! Legends say whoever holds this bad boy is basically royalty.");
#endif
    }

    public void RequestDogComment(string context)
    {
        string prompt = $"You are a small adventurous dog in a digging game. React to this situation in ONE short casual sentence (max 15 words), as a dog would: {context}. Use dog personality but speak English.";
#if UNITY_WEBGL && !UNITY_EDITOR
        CallAI("OnDogCommentReady", prompt);
#else
        OnDogCommentReady(GetEditorDogComment(context));
#endif
    }

    // Callbacks dari JS
    public void OnArtifactLoreReady(string lore)
    {
        if (ArtifactLoreUI.Instance != null)
            ArtifactLoreUI.Instance.ShowLore(lore);
    }

    public void OnDogCommentReady(string comment)
    {
        if (DogCompanion.Instance != null)
            DogCompanion.Instance.ShowComment(comment);
    }

    public void OnAIError(string error)
    {
        Debug.LogWarning("[AI] Error: " + error);
    }

    string GetEditorDogComment(string context)
    {
        if (context.Contains("deep")) return "Whoa it's dark down here, but I'm not scared!";
        if (context.Contains("energy")) return "I'm running out of steam... need a snack!";
        if (context.Contains("artifact")) return "I smell something ancient and awesome!";
        if (context.Contains("game over")) return "Oof... maybe we dug too deep this time.";
        if (context.Contains("surface")) return "Fresh air! Let's restock and go again!";
        return "Let's dig deeper, I can feel something!";
    }
}
