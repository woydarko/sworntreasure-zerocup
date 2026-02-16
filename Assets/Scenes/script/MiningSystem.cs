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

    [Header("Energy System")]
    public TileBase energyTile; // Masukkan aset tile penambah energi di sini
    public int energyRestoreAmount = 10; // Berapa banyak durabilitas yang kembali
    [Range(0, 100)] public float energySpawnChance = 5f; // Peluang munculnya (misal 5%)

    [Header("Mining Delay System")]
    public float initialDelay = 0.2f; 
    public float repeatRate = 0.08f;  

    [Header("Durability System")]
    public int maxDurability = 50;
    public int currentDurability;
    public Text durabilityText; 
    public int depthThreshold = -20;

    [Header("Teleport & Reset")]
    public Vector3 spawnPosition = new Vector3(0, 0, 0);
    public TileBase soilTile;   // Slot untuk tanah biasa
    public TileBase grassTile;  // Slot untuk tanah berrumput
    public int surfaceY = -1;   // Koordinat Y untuk rumput
    public int maxDepth = -50;  // Kedalaman maksimal reset

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
        ResetMap();
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
    }

    void ResetMap()
    {
        int width = maxX - minX + 1;
        int height = Mathf.Abs(maxDepth) + 6; 
        
        BoundsInt bounds = new BoundsInt(minX, maxDepth, 0, width, height, 1);
        TileBase[] tileArray = new TileBase[bounds.size.x * bounds.size.y * bounds.size.z];

        int index = 0;

        for (int z = 0; z < bounds.size.z; z++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                int currentWorldY = bounds.yMin + y;

                for (int x = 0; x < bounds.size.x; x++)
                {
                    if (currentWorldY == surfaceY)
                    {
                        tileArray[index] = grassTile;
                    }
                    else if (currentWorldY < surfaceY)
                    {
                        // MODIFIKASI: Logika acak untuk memunculkan Energy Tile
                        float roll = Random.Range(0f, 100f);
                        if (roll <= energySpawnChance && energyTile != null)
                        {
                            tileArray[index] = energyTile;
                        }
                        else
                        {
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
        }

        groundTilemap.ClearAllTiles();
        groundTilemap.SetTilesBlock(bounds, tileArray);
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
            // Ambil info Tile sebelum dihancurkan
            TileBase hitTile = groundTilemap.GetTile(targetGrid);

            targetPosition = targetWorldPos;
            yield return new WaitForSeconds(0.05f);

            // MODIFIKASI: Cek apakah yang dipukul adalah Energy Tile
            if (hitTile == energyTile && energyTile != null)
            {
                currentDurability += energyRestoreAmount;
                // Pastikan tidak melebihi batas maksimal
                if (currentDurability > maxDurability) currentDurability = maxDurability;
            }
            else
            {
                int cost = (targetGrid.y < depthThreshold) ? 2 : 1;
                currentDurability -= cost;
            }

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
        durabilityText.text = $"{currentDurability}";
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