using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerFactory : UpdateableGameObject
{
    public Tilemap towerSlotTilemap;
    public GameObject towerPrefab;
    public Camera cam;
    Tower[] spawnedTowers;
    int spawnedTowerIndex = 0;

    Vector3Int[] allTowerSlots;
    int[] availableTowerSlots;
    int availableSlotCount = 0;
    int lastAvailableSlotIndex = 0;

     
    private void Awake()
    {
        PrepareFactory();
    }

    private void Start()
    {
        GameManager.instance.AddUpdateableObject(this);
    }

    public override void UpdateEveryFrame()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.instance.upgPanel.gameObject.activeSelf == false)
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = towerSlotTilemap.WorldToCell(worldPos);

            for (int i = 0; i < allTowerSlots.Length; i++)
            {
                if(cell.x == allTowerSlots [i].x && cell.y == allTowerSlots[i].y)
                {
                    if (spawnedTowers[i])
                    {
                        GameManager.instance.upgPanel.PreparePanel(spawnedTowers[i]);
                    }
                }
            }
        }
    }

    public override void UpdateFixedFrame()
    {
        throw new NotImplementedException();
    }

    private void PrepareFactory()
    {
        List<Vector3Int> filledTiles = new List<Vector3Int>();

        Vector3Int tilemapOrigin = towerSlotTilemap.origin;

        for (int i = tilemapOrigin.x; i < tilemapOrigin.x + towerSlotTilemap.size.x; i++)
        {
            for (int j = tilemapOrigin.y; j < tilemapOrigin.y + towerSlotTilemap.size.y; j++)
            {
                Vector3Int tileIndices = new Vector3Int(i, j, 0);
                if ((Tile)towerSlotTilemap.GetTile(tileIndices))
                {
                    filledTiles.Add(tileIndices);
                }
            }
        }

        allTowerSlots = filledTiles.ToArray();

        spawnedTowers = new Tower[allTowerSlots.Length];
        spawnedTowerIndex = 0;

        availableTowerSlots = new int[allTowerSlots.Length];
        availableSlotCount = availableTowerSlots.Length;
        lastAvailableSlotIndex = Mathf.Max(availableTowerSlots.Length - 1, 0);

        for (int i = 0; i < availableTowerSlots.Length; i++)
        {
            availableTowerSlots[i] = i;
        }

    }

    public void SpawnTowerOnRandSlot()
    {
        if (towerSlotTilemap == null|| allTowerSlots == null || towerPrefab == null)
            return;

        Vector3Int randSlotPosition;
        int randSlotIndex = 0;
        if (TryToGetRandAvailableSlot(out randSlotPosition, out randSlotIndex))
        {
            Vector3 slotWorldPos = towerSlotTilemap.GetCellCenterWorld(randSlotPosition);
            SpawnTowerAtPos(slotWorldPos, randSlotIndex);
        }

    }
    private bool TryToGetRandAvailableSlot(out Vector3Int randSlotPosition, out int randSlotIndex)
    {
        if (lastAvailableSlotIndex < 0 || availableTowerSlots == null)
        {
            randSlotPosition = Vector3Int.zero;
            randSlotIndex = -1;
            return false;
        }

        int randSlotIndx = UnityEngine.Random.Range(0, availableSlotCount);
        int selectedTowerSlot = availableTowerSlots[randSlotIndx];

        availableTowerSlots[randSlotIndx] = availableTowerSlots[lastAvailableSlotIndex];
        availableTowerSlots[lastAvailableSlotIndex] = -1;
        lastAvailableSlotIndex -= 1;
        availableSlotCount -= 1;


        randSlotPosition = allTowerSlots[selectedTowerSlot];
        randSlotIndex = selectedTowerSlot;
        return true;
    }

    private void SpawnTowerAtPos(Vector3 pos, int slotIndex)
    {
        if (towerPrefab == null || spawnedTowers == null)
            return;

        if(spawnedTowerIndex < spawnedTowers.Length)
        {
            spawnedTowers[slotIndex] = Instantiate(towerPrefab, pos, Quaternion.identity, null).GetComponent<Tower>();
            GameManager.instance.AddUpdateableObject(spawnedTowers[slotIndex]);
            spawnedTowerIndex++;
        }
    }

    

    public int GetSpawnedTowerAmount()
    {
        return spawnedTowerIndex;

    }

    public int GetAvailableTowerSlotAmount()
    {
        return availableSlotCount;
    }

    
}
