using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SatisfyingMiner : MonoBehaviour
{
    public Tilemap groundTilemap;
    public float moveSpeed = 15f;
    
    [Header("Boundary Settings")]
    public int minX = -10;
    public int maxX = 9;
    [Header("UI Windows")]
public GameObject shopPanel; // Tarik objek ShopPanel kamu ke sini
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
    public GameObject alertPanel; // Seret objek Panel Alert kamu ke sini
    public TMP_Text alertText;    // Seret teks di dalam Panel Alert ke sini
    public float alertDuration = 2f; // Berapa lama alert muncul

    [Header("Teleport & Reset")]
    public Vector3 spawnPosition = new Vector3(0, 0, 0);
    public TileBase soilTile;   
    public TileBase grassTile;  
    public int surfaceY = -1;   
    public int maxDepth = -200;  
[Header("Skin UI Settings")]
public TMP_Text[] skinPriceTexts;
    private Vector3 targetPosition;
    private bool isProcessing = false;
    private float nextActionTime = 0f;
    private KeyCode lastKey;
    private bool isHolding = false;

    void Start()
{
    // 1. Reset Scale
    transform.localScale = new Vector3(1.66764f, 1.66764f, 1.66764f);
    if (characterRenderer != null) originalDefaultSprite = characterRenderer.sprite;
    
    // 2. RESET SEMUA DATA (Poin, Kepemilikan Skin, dan Skin Terpilih)
    // Ini akan menghapus semua memori dari sesi sebelumnya
    PlayerPrefs.DeleteAll(); 
    for (int i = 0; i < skinPriceTexts.Length; i++)
        {
            if (skinPriceTexts[i] != null)
                skinPriceTexts[i].text = skinPrices[i] + " Points";
        }
    
    // 3. Inisialisasi Ulang Variabel
    currentPoints = 0;
    currentDurability = maxDurability;
    currentSkinIndex = 0; // Kembali ke skin default
    if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }

    // 4. Update Visual & Map
    UpdateUI();
    SnapToGrid();
    ResetMap();
    
    // Karena sudah di-DeleteAll, LoadSavedSkin akan otomatis memuat index 0 (default)
    characterRenderer.sprite = originalDefaultSprite;
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
    ToggleShop();
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
    
    // LOG BARU UNTUK CEK:
    Debug.Log($"MAP GENERATED: Dari Y {bounds.yMin} sampai {bounds.yMax}. Total Height: {bounds.size.y}");
}

    public void TeleportToTop()
    {
        StopAllCoroutines();
        isProcessing = false;
        targetPosition = spawnPosition;
        transform.position = spawnPosition;
        ResetMap();
        currentDurability = maxDurability;
        UpdateUI();
    }

    void HandleInput()
    {
        if (isShopOpen) return;
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

            if (goldTile != null && hitTile == goldTile) AddPoints(5);
            else if (diamondTile != null && hitTile == diamondTile) AddPoints(20);

            UpdateUI();
            groundTilemap.SetTile(targetGrid, null);
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

    // --- SKIN SYSTEM ---
    public void BuyOrSelectSkin(int index)
{
    PlaySFX(buttonClickSound);
    Debug.Log("Mencoba beli/pilih skin index: " + index);

    bool isOwned = PlayerPrefs.GetInt(SKIN_OWNED_KEY + index, 0) == 1;
    if (isOwned)
        {
            PlaySFX(buySuccessSound); // Suara ganti skin
            EquipSkin(index);
        }
    if (isOwned)
    {
        EquipSkin(index);
        // Pastikan semua yang sudah dimiliki tulisannya "OWNED"
        skinPriceTexts[index].text = "OWNED"; 
    }
    else if (currentPoints >= skinPrices[index])
    {
        PlaySFX(buySuccessSound);
        currentPoints -= skinPrices[index];
        PlayerPrefs.SetInt(SAVE_KEY, currentPoints);
        PlayerPrefs.SetInt(SKIN_OWNED_KEY + index, 1);
        
        // LANGSUNG UBAH TEKS JADI OWNED
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
            // TAMPILKAN ALERT KARENA POIN TIDAK CUKUP
            ShowAlert("Points tidak cukup");
        }
}

    void EquipSkin(int index)
{
    if (characterRenderer != null && index < availableSkins.Length)
    {
        currentSkinIndex = index;
        characterRenderer.sprite = availableSkins[index]; // Ini yang mengubah visualnya
        transform.localScale = new Vector3(1.66764f, 1.66764f, 1.66764f);
        PlayerPrefs.SetInt(SELECTED_SKIN_KEY, index);
        
        // Debug untuk memastikan fungsi terpanggil
        Debug.Log("Sprite berubah menjadi skin ke-" + index);
    }
    else
    {
        Debug.LogError("Gagal ganti skin! Cek apakah characterRenderer sudah diisi di Inspector.");
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
    if (!isShopOpen && transform.position.y < (surfaceY - 0.5f)) 
    {
        Debug.Log("Kamu harus kembali ke permukaan untuk membuka Shop!");
        return;
    }
    isShopOpen = !isShopOpen;
    shopPanel.SetActive(isShopOpen);

    // Opsional: Hentikan galian kalau shop lagi buka biar gak mati durab
    if (isShopOpen)
    {
        isProcessing = false;
        StopAllCoroutines();
    }
}
public void ShowAlert(string message)
    {
        if (alertPanel != null && alertText != null)
        {
            StopAllCoroutines(); // Hentikan alert sebelumnya jika ada yang sedang jalan
            StartCoroutine(AlertRoutine(message));
        }
    }
    IEnumerator AlertRoutine(string message)
    {
        alertText.text = message;
        alertPanel.SetActive(true);
        
        yield return new WaitForSeconds(alertDuration);
        
        alertPanel.SetActive(false);
    }
    void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
}