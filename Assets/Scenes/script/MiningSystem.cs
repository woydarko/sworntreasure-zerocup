using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI; // Wajib untuk UI
using System.Collections;

public class SatisfyingMiner : MonoBehaviour
{
    public Tilemap groundTilemap;
    public float moveSpeed = 15f;
    
    [Header("Boundary Settings")]
    public int minX = -10;
    public int maxX = 9;

    [Header("Mining Delay System")]
    public float initialDelay = 0.3f;
    public float repeatRate = 0.1f;

    [Header("Durability System")]
    public int maxDurability = 50;
    public int currentDurability;
    public Text durabilityText; // Tarik objek UI Text ke sini nanti
    public int depthThreshold = -20; // Di bawah kedalaman ini, makan 2 poin

    private Vector3 targetPosition;
    private bool isProcessing = false;
    private float nextActionTime = 0f;
    private KeyCode lastKey;
    private bool isHolding = false;

    void Start()
    {
        currentDurability = maxDurability;
        UpdateUI();
        SnapToGrid();
    }

    void Update()
{
    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

    if (!isProcessing && Vector3.Distance(transform.position, targetPosition) < 0.01f)
    {
        // Langsung panggil HandleInput tanpa dibungkus cek durability
        HandleInput();
    }
}

    void HandleInput()
    {
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
    // 1. Tentukan koordinat tujuan
    Vector3Int targetGrid = groundTilemap.WorldToCell(transform.position) + direction;
    
    // 2. CEK BATAS: Jika mau keluar frame X, langsung stop
    if (targetGrid.x < minX || targetGrid.x > maxX) return;

    // 3. LOGIKA FILTER: Cek apakah tujuan itu TANAH atau KOSONG
    bool isTargetTile = groundTilemap.HasTile(targetGrid);

    // Jika tujuannya adalah TANAH tapi durability habis, kita blokir di sini
    if (isTargetTile && currentDurability <= 0)
    {
        return; 
    }

    // 4. JIKA LOLOS FILTER (Berarti: Tujuannya Kosong ATAU Durability masih ada)
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

        if (groundTilemap.HasTile(targetGrid))
        {
            // Double check: pastikan masih ada durability tepat sebelum gali
            int cost = (targetGrid.y < depthThreshold) ? 2 : 1;
            
            if (currentDurability >= cost)
            {
                targetPosition = targetWorldPos;
                while (Vector3.Distance(transform.position, targetPosition) > 0.15f) yield return null;

                currentDurability -= cost;
                if (currentDurability < 0) currentDurability = 0;
                UpdateUI();

                groundTilemap.SetTile(targetGrid, null);
            }
        }
        else
        {
            // Jika grid kosong, langsung pindah saja tanpa potong durability
            targetPosition = targetWorldPos;
        }

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f) yield return null;

        CheckGravity();
        isProcessing = false;
    }

    void UpdateUI()
{
    if (durabilityText != null)
    {
        // Menampilkan sisa durability
        durabilityText.text = "Durability: " + currentDurability + " / " + maxDurability;

        // EFEK VISUAL: Berubah merah kalau sisa 1 digit (dibawah 10)
        if (currentDurability < 10)
        {
            durabilityText.color = Color.red;
            durabilityText.text = $"Durability: {currentDurability:00}";
            // Opsi: Jika mau menghilangkan "/" saat sisa 1 digit, aktifkan baris bawah ini:
            // durabilityText.text = "LOW DURABILITY: " + currentDurability;
        }
        else
        {
            durabilityText.color = Color.white;
        }
    }
}

    // Sisanya (CheckGravity & SnapToGrid) tetap sama seperti sebelumnya
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
}