using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SatisfyingMiner : MonoBehaviour
{
    [System.Serializable]
    class ArtifactData
    {
        public string rarity;
        public string artifactName;
        [TextArea(2, 4)] public string description;
        public TileBase tile;
        public int rewardPoints;
        public int recommendedDepthMin;
        public int recommendedDepthMax;
        [Range(0, 100)] public float idealChance;
        [Range(0, 100)] public float outsideChance;
        public string rarityColorHex;

        public bool HasTile => tile != null;

        public bool IsInRecommendedDepth(int depth)
        {
            return depth >= recommendedDepthMin && depth <= recommendedDepthMax;
        }

        public float GetChance(int depth)
        {
            if (depth < recommendedDepthMin)
            {
                return 0f;
            }

            return IsInRecommendedDepth(depth) ? idealChance : outsideChance;
        }
    }

    public Tilemap groundTilemap;
    public float moveSpeed = 15f;
    
    [Header("Boundary Settings")]
    public int minX = -10;
    public int maxX = 9;
    [Header("UI Windows")]
    public GameObject shopPanel;
    private bool isShopOpen = false;

    [Header("Energy System")]
    public TileBase energyTile; 
    public int energyRestoreAmount = 10; 
    [Range(0, 100)] public float energySpawnChance = 0.2f; 
    [Header("Audio Settings")]
    public AudioSource bgmSource;      // Untuk musik latar (Looping)
    public AudioSource sfxSource;      // Untuk suara sekali bunyi (SFX)
    
    public AudioClip digSound;         // Suara saat menggali tanah
    public AudioClip buttonClickSound; // Suara saat klik tombol umum
    public AudioClip buySuccessSound;  // Suara saat berhasil beli/ganti skin
    public AudioClip alertErrorSound;  // Suara saat poin tidak cukup
    public AudioClip[] artifactBarkSounds; // Suara gonggong saat menemukan artefak

    [Header("Mining Delay System")]
    public float initialDelay = 0.2f; 
    public float repeatRate = 0.08f;  

    [Header("Economy System")]
    public TileBase goldTile;
    [Range(0, 100)] public float goldSpawnChance = 5f; 
    public TileBase diamondTile;
    [Range(0, 100)] public float diamondSpawnChance = 1f;
    public int currentPoints;
    public Text scoreText;
    private string SAVE_KEY = "TotalMoney";

    [Header("Durability System")]
    public int maxDurability = 50;
    public int currentDurability;
    public Text durabilityText; 
    public int depthThreshold = -20;
    public int energyPurchaseCost = 50;
    public int energyPurchaseAmount = 50;

    [Header("Skin System")]
    public SpriteRenderer characterRenderer; 
    public Sprite[] availableSkins; 
    public int[] skinPrices = { 25, 50, 75 }; 
    private Sprite originalDefaultSprite;
    private int currentSkinIndex = 0;
    // Variabel kunci untuk PlayerPrefs agar tidak Error CS0103
    private string SKIN_OWNED_KEY = "SkinOwned_";
    private string SELECTED_SKIN_KEY = "SelectedSkin";
    [Header("UI Pop-up Settings")]
    public GameObject alertPanel;
    public TMP_Text alertText;
    public float alertDuration = 2f;
    private Coroutine alertCoroutine;

    [Header("Teleport & Reset")]
    public Vector3 spawnPosition = new Vector3(0, 0, 0);
    public TileBase soilTile;   
    public TileBase grassTile;  
    public int surfaceY = -1;   
    public int maxDepth = -300;  
    public int backToTopMinimumDepth = 50;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverScoreText;
    private bool isGameOver = false;

    [Header("Artifact System")]
    public TileBase[] artifactTiles = new TileBase[5];
    private ArtifactData[] artifacts;

    [Header("Bag UI")]
    public bool createBagUIOnStart = false;
    public GameObject bagPanel;
    private Button bagButton;
    private bool isBagOpen = false;

    [Header("Dig Effect")]
    public bool enableDigParticles = true;
    public Color digParticleColor = new Color(0.58f, 0.36f, 0.18f, 0.82f);
    [Range(1, 20)] public int digParticleCount = 7;
    private ParticleSystem digParticleSystem;
[Header("Skin UI Settings")]
public TMP_Text[] skinPriceTexts;
    private Vector3 targetPosition;
    private bool isProcessing = false;
    private float nextActionTime = 0f;
    private KeyCode lastKey;
    private bool isHolding = false;
    private readonly Color panelColor = new Color(0.176f, 0.106f, 0.078f, 0.96f);
    private readonly Color buttonColor = new Color(0.973f, 0.773f, 0.404f, 1f);
    private readonly Color buttonHoverColor = new Color(1f, 0.941f, 0.82f, 1f);
    private readonly Color buttonPressedColor = new Color(0.827f, 0.612f, 0.345f, 1f);
    private readonly Color darkTextColor = new Color(0.176f, 0.106f, 0.078f, 1f);
    private readonly Color lightTextColor = new Color(1f, 0.949f, 0.741f, 1f);

    private bool firstDigDone = false;
    private int lastDepthMilestone = 0;
    private bool lowEnergyTriggered = false;

    void Start()
{
    transform.localScale = new Vector3(1.66764f, 1.66764f, 1.66764f);
    if (characterRenderer != null) originalDefaultSprite = characterRenderer.sprite;
    
    // Preserve 0G tx hashes across resets
    string savedZeroGHashes = PlayerPrefs.GetString("ZeroG_Hashes", "");
    PlayerPrefs.DeleteAll();
    if (!string.IsNullOrEmpty(savedZeroGHashes))
        PlayerPrefs.SetString("ZeroG_Hashes", savedZeroGHashes);
    for (int i = 0; i < skinPriceTexts.Length; i++)
        {
            if (skinPriceTexts[i] != null)
                skinPriceTexts[i].text = skinPrices[i] + " Points";
        }
    
    currentPoints = 0;
    currentDurability = maxDurability;
    currentSkinIndex = 0; // Kembali ke skin default
    if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }

    InitializeArtifactDefaults();
    ConfigureAlertAsNonBlocking();

    if (createBagUIOnStart)
    {
        CreateBagUI();
    }
    CreateDigParticleSystem();

    UpdateUI();
    SnapToGrid();
    ResetMap();

    characterRenderer.sprite = originalDefaultSprite;

    // Ensure the in-game on-chain submit button exists
    if (FindObjectOfType<InGameSubmitButton>() == null)
    {
        GameObject submitGO = new GameObject("InGameSubmitButton");
        submitGO.AddComponent<InGameSubmitButton>();
    }
}

    void Update()
    {
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        if (!isProcessing && Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            HandleInput();
        }
        if (Input.GetKeyDown(KeyCode.B))
{
    ToggleBag();
}
    }

    void ResetMap()
{
    // Pastikan kita menggunakan nilai terbaru dari Inspector
    // Jika maxDepth adalah -100, maka height akan menyesuaikan
    int width = maxX - minX + 1;
    
    // Perhitungan tinggi yang absolut dari permukaan ke titik terdalam
    int height = Mathf.Abs(surfaceY - maxDepth) + 10; 

    // Tentukan bounds mulai dari titik paling negatif (maxDepth)
    BoundsInt bounds = new BoundsInt(minX, maxDepth, 0, width, height, 1);
    
    TileBase[] tileArray = new TileBase[bounds.size.x * bounds.size.y * bounds.size.z];

    int index = 0;
    for (int y = 0; y < bounds.size.y; y++)
    {
        for (int x = 0; x < bounds.size.x; x++)
        {
            int currentWorldY = bounds.yMin + y;

            if (currentWorldY == surfaceY)
            {
                tileArray[index] = grassTile;
            }
            else if (currentWorldY < surfaceY)
            {
                // Bagian ini yang mengisi tanah (Foreground)
                int depth = Mathf.Max(0, surfaceY - currentWorldY);
                TileBase artifactTile;

                if (TryGetArtifactTile(depth, out artifactTile))
                {
                    tileArray[index] = artifactTile;
                }
                else
                {
                    float roll = Random.Range(0f, 100f);

                    if (roll <= diamondSpawnChance && diamondTile != null) 
                    tileArray[index] = diamondTile;
                    else if (roll <= (diamondSpawnChance + goldSpawnChance) && goldTile != null) 
                    tileArray[index] = goldTile;
                    else if (roll <= (diamondSpawnChance + goldSpawnChance + energySpawnChance) && energyTile != null) 
                    tileArray[index] = energyTile;
                    else 
                    tileArray[index] = soilTile;
                }
            }
            else
            {
                tileArray[index] = null;
            }
            index++;
        }
    }

    groundTilemap.ClearAllTiles();
    groundTilemap.SetTilesBlock(bounds, tileArray);
    
    // Paksa update collider dan visual
    groundTilemap.CompressBounds();
    groundTilemap.RefreshAllTiles();
    
}

    public void TeleportToTop()
    {
        int currentDepth = GetCurrentDepth();
        if (currentDepth <= 0)
        {
            PlaySFX(alertErrorSound);
            ShowAlert("You're already on the surface!");
            return;
        }

        if (currentDurability > 0 && currentDepth <= backToTopMinimumDepth)
        {
            PlaySFX(alertErrorSound);
            ShowAlert($"Back to Top only works when energy is empty or depth > {backToTopMinimumDepth}!");
            return;
        }

        HideAlertImmediate();
        StopAllCoroutines();
        isProcessing = false;
        targetPosition = spawnPosition;
        transform.position = spawnPosition;
        ResetMap();
        UpdateUI();

        // No auto-submit. Players must use the in-game "Submit Score" button before they risk digging again.
        if (DogCompanion.Instance != null) DogCompanion.Instance.OnBackToSurface();
        lowEnergyTriggered = false;
        lastDepthMilestone = 0;
        firstDigDone = false;
    }

    void HandleInput()
    {
        if (isGameOver) return;
        if (isShopOpen) return;
        if (isBagOpen) return;
        if (Input.GetKey(KeyCode.S)) ProcessMiningInput(KeyCode.S, Vector3Int.down);
        else if (Input.GetKey(KeyCode.A)) ProcessMiningInput(KeyCode.A, Vector3Int.left);
        else if (Input.GetKey(KeyCode.D)) ProcessMiningInput(KeyCode.D, Vector3Int.right);
        else
        {
            isHolding = false;
            nextActionTime = 0;
        }
    }

    void ProcessMiningInput(KeyCode key, Vector3Int direction)
    {
        Vector3Int targetGrid = groundTilemap.WorldToCell(transform.position) + direction;
        if (targetGrid.x < minX || targetGrid.x > maxX) return;

        bool isTargetTile = groundTilemap.HasTile(targetGrid);
        if (isTargetTile && currentDurability <= 0) return; 

        if (Time.time >= nextActionTime)
        {
            if (!isHolding || lastKey != key)
            {
                isHolding = true;
                lastKey = key;
                nextActionTime = Time.time + initialDelay;
            }
            else
            {
                nextActionTime = Time.time + repeatRate;
            }
            StartCoroutine(MoveAndMine(direction));
        }
    }

    IEnumerator MoveAndMine(Vector3Int direction)
    {
        isProcessing = true;
        Vector3Int targetGrid = groundTilemap.WorldToCell(transform.position) + direction;
        Vector3 targetWorldPos = groundTilemap.GetCellCenterWorld(targetGrid);
        TileBase hitTile = groundTilemap.GetTile(targetGrid);

        if (groundTilemap.HasTile(targetGrid))
        {
            PlaySFX(digSound);
            PlayDigParticles(targetWorldPos);
            targetPosition = targetWorldPos;
            yield return new WaitForSeconds(0.05f);

            if (energyTile != null && hitTile == energyTile)
            {
                currentDurability += energyRestoreAmount;
                if (currentDurability > maxDurability) currentDurability = maxDurability;
            }
            else
            {
                int cost = (targetGrid.y < depthThreshold) ? 2 : 1;
                currentDurability -= cost;
            }

            if (currentDurability < 0) currentDurability = 0;

            ArtifactData foundArtifact = GetArtifactByTile(hitTile);
            if (foundArtifact != null)
            {
                AddPoints(foundArtifact.rewardPoints);
                PlayArtifactBark();
                PlaySFX(buySuccessSound);
                ShowAlert($"Found <color={foundArtifact.rarityColorHex}>{foundArtifact.artifactName}</color> +{foundArtifact.rewardPoints} pts!");

                if (AIService.Instance != null)
                    AIService.Instance.RequestArtifactLore(foundArtifact.artifactName, foundArtifact.rarity, foundArtifact.description);
                if (ArtifactLoreUI.Instance != null)
                    ArtifactLoreUI.Instance.ShowPending(foundArtifact.artifactName);
                if (DogCompanion.Instance != null)
                    DogCompanion.Instance.OnArtifactFound(foundArtifact.artifactName);
            }
            else if (goldTile != null && hitTile == goldTile) AddPoints(5);
            else if (diamondTile != null && hitTile == diamondTile) AddPoints(20);

            // First dig trigger
            if (!firstDigDone)
            {
                firstDigDone = true;
                if (DogCompanion.Instance != null) DogCompanion.Instance.OnFirstDig();
            }

            // Depth milestone trigger (every 50 depth)
            int depth = GetCurrentDepth();
            int milestone = (depth / 50) * 50;
            if (milestone > 0 && milestone != lastDepthMilestone)
            {
                lastDepthMilestone = milestone;
                if (DogCompanion.Instance != null) DogCompanion.Instance.OnDeepReached(depth);
            }

            // Low energy trigger (below 20%)
            float energyPercent = (float)currentDurability / maxDurability;
            if (energyPercent < 0.2f && !lowEnergyTriggered)
            {
                lowEnergyTriggered = true;
                if (DogCompanion.Instance != null) DogCompanion.Instance.OnLowEnergy(currentDurability);
            }
            else if (energyPercent >= 0.2f)
            {
                lowEnergyTriggered = false;
            }

            UpdateUI();
            groundTilemap.SetTile(targetGrid, null);
            CheckGameOver();
        }
        else
        {
            targetPosition = targetWorldPos;
        }

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f) yield return null;
        CheckGravity();
        isProcessing = false;
    }

    void AddPoints(int amount) 
    {
        currentPoints += amount;
        PlayerPrefs.SetInt(SAVE_KEY, currentPoints);
        PlayerPrefs.Save();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (durabilityText != null) durabilityText.text = $"{currentDurability}";
        if (scoreText != null) scoreText.text = $"Points: {currentPoints}";
    }

    void CheckGameOver()
    {
        if (isGameOver) return;
        if (currentDurability > 0) return;
        if (currentPoints >= energyPurchaseCost) return;

        isGameOver = true;
        isProcessing = true;
        HideAlertImmediate();
        StopAllCoroutines();

        // No auto-submit on game over. If the player didn't submit before dying, the score is lost.
        if (DogCompanion.Instance != null) DogCompanion.Instance.OnGameOver();

        if (gameOverScoreText != null)
            gameOverScoreText.text = $"Score: {currentPoints} pts";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        isGameOver = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        currentPoints = 0;
        currentDurability = maxDurability;
        firstDigDone = false;
        lastDepthMilestone = 0;
        lowEnergyTriggered = false;
        HideAlertImmediate();
        StopAllCoroutines();
        isProcessing = false;
        targetPosition = spawnPosition;
        transform.position = spawnPosition;
        ResetMap();
        UpdateUI();
    }

    void CheckGravity()
    {
        Vector3Int belowTile = groundTilemap.WorldToCell(transform.position) + Vector3Int.down;
        if (!groundTilemap.HasTile(belowTile))
        {
            targetPosition = groundTilemap.GetCellCenterWorld(belowTile);
        }
    }

    void SnapToGrid()
    {
        Vector3Int gridPos = groundTilemap.WorldToCell(transform.position);
        targetPosition = groundTilemap.GetCellCenterWorld(gridPos);
        transform.position = targetPosition;
    }

    bool TryGetArtifactTile(int depth, out TileBase artifactTile)
    {
        artifactTile = null;
        if (artifacts == null || artifacts.Length == 0)
        {
            return false;
        }

        float totalChance = 0f;
        for (int i = 0; i < artifacts.Length; i++)
        {
            ArtifactData artifact = artifacts[i];
            if (artifact != null && artifact.HasTile)
            {
                totalChance += artifact.GetChance(depth);
            }
        }

        if (totalChance <= 0f)
        {
            return false;
        }

        float roll = Random.Range(0f, 100f);
        if (roll > totalChance)
        {
            return false;
        }

        float cumulativeChance = 0f;
        for (int i = 0; i < artifacts.Length; i++)
        {
            ArtifactData artifact = artifacts[i];
            if (artifact == null || !artifact.HasTile)
            {
                continue;
            }

            cumulativeChance += artifact.GetChance(depth);
            if (roll <= cumulativeChance)
            {
                artifactTile = artifact.tile;
                return true;
            }
        }

        return false;
    }

    ArtifactData GetArtifactByTile(TileBase tile)
    {
        if (tile == null || artifacts == null)
        {
            return null;
        }

        for (int i = 0; i < artifacts.Length; i++)
        {
            ArtifactData artifact = artifacts[i];
            if (artifact != null && artifact.tile != null && artifact.tile == tile)
            {
                return artifact;
            }
        }

        return null;
    }

    int GetCurrentDepth()
    {
        if (groundTilemap == null)
        {
            return 0;
        }

        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position);
        return Mathf.Max(0, surfaceY - gridPosition.y);
    }

    void InitializeArtifactDefaults()
    {
        ArtifactData[] defaults = new ArtifactData[]
        {
            new ArtifactData
            {
                rarity = "Common",
                artifactName = "Trowulan Pottery Shard",
                description = "A clay fragment left behind from the Majapahit Kingdom's daily life. Usually found close to the surface.",
                rewardPoints = 30,
                recommendedDepthMin = 0,
                recommendedDepthMax = 40,
                idealChance = 0.3f,
                outsideChance = 0.15f,
                rarityColorHex = "#D38A45"
            },
            new ArtifactData
            {
                rarity = "Uncommon",
                artifactName = "Gobog Wayang Coin",
                description = "An old coin with a hole in the center, engraved with wayang figures. Used as currency and a lucky charm.",
                rewardPoints = 40,
                recommendedDepthMin = 40,
                recommendedDepthMax = 80,
                idealChance = 0.2f,
                outsideChance = 0.1f,
                rarityColorHex = "#9A9A9A"
            },
            new ArtifactData
            {
                rarity = "Rare",
                artifactName = "Bronze Funnel Axe",
                description = "A prehistoric axe with a hollow socket at the top for fitting a wooden handle. Seriously old stuff.",
                rewardPoints = 60,
                recommendedDepthMin = 80,
                recommendedDepthMax = 120,
                idealChance = 0.1f,
                outsideChance = 0.08f,
                rarityColorHex = "#2F80ED"
            },
            new ArtifactData
            {
                rarity = "Ultra Rare",
                artifactName = "Bronze Ganesha Statue",
                description = "A tiny classical-era bronze figure with intricate carvings and deep oxidation marks. Way too pretty to leave underground.",
                rewardPoints = 75,
                recommendedDepthMin = 120,
                recommendedDepthMax = 150,
                idealChance = 0.075f,
                outsideChance = 0.045f,
                rarityColorHex = "#8E44AD"
            },
            new ArtifactData
            {
                rarity = "Mythical",
                artifactName = "Wonoboyo Gold Bowl",
                description = "A gold bowl engraved with Ramayana reliefs, from one of the biggest treasure finds in Central Java. Absolutely legendary.",
                rewardPoints = 100,
                recommendedDepthMin = 150,
                recommendedDepthMax = 300,
                idealChance = 0.03f,
                outsideChance = 0.0003f,
                rarityColorHex = "#D72638"
            }
        };

        artifacts = defaults;

        if (artifactTiles == null || artifactTiles.Length != defaults.Length)
        {
            TileBase[] previousTiles = artifactTiles;
            artifactTiles = new TileBase[defaults.Length];
            if (previousTiles != null)
            {
                int copyCount = Mathf.Min(previousTiles.Length, artifactTiles.Length);
                for (int i = 0; i < copyCount; i++)
                {
                    artifactTiles[i] = previousTiles[i];
                }
            }
        }

        for (int i = 0; i < artifacts.Length; i++)
        {
            artifacts[i].tile = artifactTiles[i];
        }
    }

    void CreateDigParticleSystem()
    {
        if (!enableDigParticles || digParticleSystem != null)
        {
            return;
        }

        GameObject particleObject = new GameObject("DigParticles");
        digParticleSystem = particleObject.AddComponent<ParticleSystem>();

        ParticleSystem.MainModule main = digParticleSystem.main;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = 0.32f;
        main.startSpeed = 1.35f;
        main.startSize = 0.12f;
        main.startColor = digParticleColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 80;

        ParticleSystem.EmissionModule emission = digParticleSystem.emission;
        emission.enabled = false;

        ParticleSystem.ShapeModule shape = digParticleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.12f;

        ParticleSystem.VelocityOverLifetimeModule velocity = digParticleSystem.velocityOverLifetime;
        velocity.enabled = false;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = digParticleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(digParticleColor, 0f), new GradientColorKey(digParticleColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(digParticleColor.a, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
    }

    void PlayDigParticles(Vector3 position)
    {
        if (!enableDigParticles)
        {
            return;
        }

        if (digParticleSystem == null)
        {
            CreateDigParticleSystem();
        }

        if (digParticleSystem == null)
        {
            return;
        }

        digParticleSystem.transform.position = position;
        for (int i = 0; i < digParticleCount; i++)
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            emitParams.velocity = new Vector3(Random.Range(-0.55f, 0.55f), Random.Range(0.25f, 0.85f), 0f);
            emitParams.startSize = Random.Range(0.08f, 0.14f);
            digParticleSystem.Emit(emitParams, 1);
        }
    }

    // --- SKIN SYSTEM ---
    public void BuyOrSelectSkin(int index)
{
    PlaySFX(buttonClickSound);

    bool isOwned = PlayerPrefs.GetInt(SKIN_OWNED_KEY + index, 0) == 1;
    if (isOwned)
    {
        PlaySFX(buySuccessSound);
        if (index < skinPriceTexts.Length) skinPriceTexts[index].text = "OWNED";
        EquipSkin(index);
        return;
    }
    else if (currentPoints >= skinPrices[index])
    {
        // Prevent soft-lock: if out of energy, buying this skin must not drop points
        // below the energy refill cost, or the player gets stuck (can't dig, can't refill).
        if (currentDurability <= 0 && (currentPoints - skinPrices[index]) < energyPurchaseCost)
        {
            PlaySFX(alertErrorSound);
            ShowAlert("Refill your energy first!");
            return;
        }

        PlaySFX(buySuccessSound);
        currentPoints -= skinPrices[index];
        PlayerPrefs.SetInt(SAVE_KEY, currentPoints);
        PlayerPrefs.SetInt(SKIN_OWNED_KEY + index, 1);

        if (index < skinPriceTexts.Length)
        {
            skinPriceTexts[index].text = "OWNED";
        }

        EquipSkin(index);
        UpdateUI();
    }
    else
        {
            PlaySFX(alertErrorSound);
            ShowAlert("Not enough points!");
        }
}

    void EquipSkin(int index)
{
    if (characterRenderer != null && index < availableSkins.Length)
    {
        currentSkinIndex = index;
        characterRenderer.sprite = availableSkins[index];
        transform.localScale = new Vector3(1.66764f, 1.66764f, 1.66764f);
        PlayerPrefs.SetInt(SELECTED_SKIN_KEY, index);
    }
}

    void LoadSavedSkin()
    {
        int savedSkin = PlayerPrefs.GetInt(SELECTED_SKIN_KEY, 0);
        EquipSkin(savedSkin);
    }
public void ToggleShop()
{
    PlaySFX(buttonClickSound);
    if (!isShopOpen && GetCurrentDepth() > 0) 
    {
        Debug.Log("You need to go back to the surface to open the Shop!");
        PlaySFX(alertErrorSound);
        ShowAlert("Shop is only available on the surface!");
        return;
    }

    if (!isShopOpen && isBagOpen)
    {
        SetBagOpen(false);
    }

    isShopOpen = !isShopOpen;
    if (shopPanel != null)
    {
        shopPanel.SetActive(isShopOpen);
    }

    if (isShopOpen)
    {
        isProcessing = false;
        HideAlertImmediate();
        StopAllCoroutines();
    }
}
    public void BuyEnergy()
    {
        PlaySFX(buttonClickSound);

        if (currentDurability >= maxDurability)
        {
            PlaySFX(alertErrorSound);
            ShowAlert("Energy is already full!");
            return;
        }

        if (currentPoints < energyPurchaseCost)
        {
            PlaySFX(alertErrorSound);
            ShowAlert("Not enough points!");
            return;
        }

        int previousDurability = currentDurability;
        currentPoints -= energyPurchaseCost;
        currentDurability += energyPurchaseAmount;
        if (currentDurability > maxDurability)
        {
            currentDurability = maxDurability;
        }
        int restoredAmount = currentDurability - previousDurability;

        PlayerPrefs.SetInt(SAVE_KEY, currentPoints);
        PlayerPrefs.Save();
        PlaySFX(buySuccessSound);
        UpdateUI();
        ShowAlert($"+{restoredAmount} energy restored!");
    }

    public void ToggleBag()
    {
        PlaySFX(buttonClickSound);
        SetBagOpen(!isBagOpen);
    }

    void SetBagOpen(bool open)
    {
        if (bagPanel == null)
        {
            CreateBagUI();
        }

        isBagOpen = open;
        if (bagPanel != null)
        {
            bagPanel.SetActive(isBagOpen);
        }

        if (isBagOpen)
        {
            if (isShopOpen && shopPanel != null)
            {
                isShopOpen = false;
                shopPanel.SetActive(false);
            }

            isProcessing = false;
            HideAlertImmediate();
            StopAllCoroutines();
        }
    }

    void CreateBagUI()
    {
        Canvas canvas = null;
        if (shopPanel != null)
        {
            canvas = shopPanel.GetComponentInParent<Canvas>();
        }

        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        if (canvas == null) return;

        if (bagButton == null)
        {
            bagButton = CreateUIButton("Bag", canvas.transform, "BAG", new Vector2(160f, 74f), new Vector2(1f, 1f), new Vector2(-348f, -56f));
            bagButton.onClick.AddListener(ToggleBag);
        }

        if (bagPanel == null)
        {
            bagPanel = CreateUIPanel("BagPanel", canvas.transform, new Vector2(900f, 560f), new Vector2(0.5f, 0.5f), Vector2.zero);

            TMP_Text titleText = CreateUIText("BagTitle", bagPanel.transform, "ARTIFACT GUIDE", 34f, FontStyles.Bold, TextAlignmentOptions.Center, lightTextColor);
            SetRect(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -46f), new Vector2(760f, 54f), new Vector2(0.5f, 0.5f));

            TMP_Text infoText = CreateUIText("BagInfo", bagPanel.transform, BuildArtifactGuideText(), 20f, FontStyles.Normal, TextAlignmentOptions.TopLeft, Color.white);
            infoText.enableWordWrapping = true;
            infoText.overflowMode = TextOverflowModes.Truncate;
            SetRect(infoText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, -26f), new Vector2(790f, 390f), new Vector2(0.5f, 0.5f));

            Button closeButton = CreateUIButton("BagClose", bagPanel.transform, "EXIT", new Vector2(88f, 48f), new Vector2(0.5f, 0.5f), new Vector2(390f, 245f));
            closeButton.onClick.AddListener(ToggleBag);

            bagPanel.SetActive(false);
        }
    }

    GameObject CreateUIPanel(string objectName, Transform parent, Vector2 size, Vector2 anchor, Vector2 anchoredPosition)
    {
        GameObject panel = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(parent, false);
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        SetRect(rectTransform, anchor, anchoredPosition, size, new Vector2(0.5f, 0.5f));
        Image image = panel.GetComponent<Image>();
        image.color = panelColor;
        image.raycastTarget = true;
        return panel;
    }

    Button CreateUIButton(string objectName, Transform parent, string label, Vector2 size, Vector2 anchor, Vector2 anchoredPosition)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        SetRect(rectTransform, anchor, anchoredPosition, size, new Vector2(0.5f, 0.5f));

        Image image = buttonObject.GetComponent<Image>();
        image.color = buttonColor;

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = buttonHoverColor;
        colors.pressedColor = buttonPressedColor;
        colors.selectedColor = buttonHoverColor;
        colors.disabledColor = new Color(0.784f, 0.784f, 0.784f, 0.502f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;
        button.targetGraphic = image;

        TMP_Text labelText = CreateUIText(objectName + "Text", buttonObject.transform, label, 22f, FontStyles.Bold, TextAlignmentOptions.Center, darkTextColor);
        SetRect(labelText.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));
        labelText.rectTransform.anchorMin = Vector2.zero;
        labelText.rectTransform.anchorMax = Vector2.one;
        return button;
    }

    TMP_Text CreateUIText(string objectName, Transform parent, string text, float fontSize, FontStyles fontStyle, TextAlignmentOptions alignment, Color color)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        TMP_Text textComponent = textObject.GetComponent<TMP_Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.fontStyle = fontStyle;
        textComponent.alignment = alignment;
        textComponent.color = color;
        textComponent.raycastTarget = false;
        textComponent.enableAutoSizing = true;
        textComponent.fontSizeMin = Mathf.Max(12f, fontSize - 8f);
        textComponent.fontSizeMax = fontSize;

        return textComponent;
    }

    void SetRect(RectTransform rectTransform, Vector2 anchor, Vector2 anchoredPosition, Vector2 size, Vector2 pivot)
    {
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;
        rectTransform.pivot = pivot;
        rectTransform.localScale = Vector3.one;
    }

    string BuildArtifactGuideText()
    {
        string guide = "";
        if (artifacts == null)
        {
            return guide;
        }

        for (int i = 0; i < artifacts.Length; i++)
        {
            ArtifactData artifact = artifacts[i];
            if (artifact == null)
            {
                continue;
            }

            guide += $"<color={artifact.rarityColorHex}>{artifact.rarity}</color> - {artifact.artifactName}\n";
            guide += $"Depth: {artifact.recommendedDepthMin}-{artifact.recommendedDepthMax} | Chance: {artifact.idealChance}% | Outside range: {artifact.outsideChance}% | Reward: +{artifact.rewardPoints} pts\n";
            if (artifact.recommendedDepthMin > 0)
            {
                guide += $"Before depth {artifact.recommendedDepthMin}: 0% chance.\n";
            }
            guide += $"{artifact.description}\n\n";
        }

        return guide.TrimEnd();
    }

    public void ShowAlert(string message)
    {
        if (alertPanel != null && alertText != null)
        {
            if (alertCoroutine != null)
            {
                StopCoroutine(alertCoroutine);
            }

            alertCoroutine = StartCoroutine(AlertRoutine(message));
        }
    }

    void ConfigureAlertAsNonBlocking()
    {
        if (alertPanel == null)
        {
            return;
        }

        CanvasGroup canvasGroup = alertPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = alertPanel.AddComponent<CanvasGroup>();
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Graphic[] graphics = alertPanel.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].raycastTarget = false;
        }
    }

    void HideAlertImmediate()
    {
        if (alertCoroutine != null)
        {
            StopCoroutine(alertCoroutine);
            alertCoroutine = null;
        }

        if (alertPanel != null)
        {
            alertPanel.SetActive(false);
        }
    }

    IEnumerator AlertRoutine(string message)
    {
        ConfigureAlertAsNonBlocking();
        alertText.text = message;
        alertPanel.SetActive(true);
        
        yield return new WaitForSeconds(alertDuration);
        
        alertPanel.SetActive(false);
        alertCoroutine = null;
    }
    void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    void PlayArtifactBark()
    {
        if (artifactBarkSounds == null || artifactBarkSounds.Length == 0)
        {
            return;
        }

        AudioClip clip = artifactBarkSounds[Random.Range(0, artifactBarkSounds.Length)];
        PlaySFX(clip);
    }
    
}
