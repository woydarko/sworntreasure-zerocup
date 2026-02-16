using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections;

public class SatisfyingMiner : MonoBehaviour
{
    public Tilemap groundTilemap;
    public float moveSpeed = 15f;
    
    [Header("Boundary Settings")]
    public int minX = -10;
    public int maxX = 9;

    [Header("Mining Delay System")]
    public float initialDelay = 0.2f; // Dipercepat dikit biar responsif
    public float repeatRate = 0.08f;  // Kecepatan spam yang lebih optimal

    [Header("Durability System")]
    public int maxDurability = 50;
    public int currentDurability;
    public Text durabilityText; 
    public int depthThreshold = -20;

    [Header("Teleport & Reset")]
    public Vector3 spawnPosition = new Vector3(0, 0, 0);
    public TileBase soilTile;

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
        // Gunakan interpolasi yang lebih stabil
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        if (!isProcessing && Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            HandleInput();
        }
    }

    // OPTIMASI: SetTilesBlock jauh lebih ringan daripada looping SetTile
    void ResetMap()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        TileBase[] tileArray = new TileBase[bounds.size.x * bounds.size.y * bounds.size.z];

        for (int i = 0; i < tileArray.Length; i++)
        {
            tileArray[i] = soilTile;
        }

        groundTilemap.SetTilesBlock(bounds, tileArray);
        // Memaksa refresh tilemap hanya sekali
        groundTilemap.RefreshAllTiles();
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

        if (groundTilemap.HasTile(targetGrid))
        {
            targetPosition = targetWorldPos;
            
            // Tunggu sedikit saja biar berasa nabrak
            yield return new WaitForSeconds(0.05f);

            int cost = (targetGrid.y < depthThreshold) ? 2 : 1;
            currentDurability -= cost;
            if (currentDurability < 0) currentDurability = 0;
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

    void UpdateUI()
    {
        if (durabilityText == null) return;

        durabilityText.text = $"Durability: {currentDurability} / {maxDurability}";
        durabilityText.color = (currentDurability < 10) ? Color.red : Color.white;
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
}