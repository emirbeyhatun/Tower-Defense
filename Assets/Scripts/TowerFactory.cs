using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerFactory : MonoBehaviour
{
    public Tilemap towerSlotTilemap;
    public GameObject towerPrefab;

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
        if (TryToGetRandAvailableSlot(out randSlotPosition))
        {
            Vector3 slotWorldPos = towerSlotTilemap.GetCellCenterWorld(randSlotPosition);
            SpawnTowerAtPos(slotWorldPos);
        }

    }

    private void SpawnTowerAtPos(Vector3 pos)
    {
        if (towerPrefab == null || spawnedTowers == null)
            return;

        if(spawnedTowerIndex < spawnedTowers.Length)
        {
            spawnedTowers[spawnedTowerIndex] = Instantiate(towerPrefab, pos, Quaternion.identity, null).GetComponent<Tower>();
            GameManager.instance.AddUpdateableObject(spawnedTowers[spawnedTowerIndex]);

            spawnedTowerIndex++;
        }
    }

    private bool TryToGetRandAvailableSlot(out Vector3Int randSlotPosition)
    {
        if (lastAvailableSlotIndex < 0 || availableTowerSlots == null || availableTowerSlots == null)
        {
            randSlotPosition = Vector3Int.zero;
            return false;
        }

        int randSlotIndx = UnityEngine.Random.Range(0, availableSlotCount);
        int selectedTowerSlot = availableTowerSlots[randSlotIndx];

        availableTowerSlots[randSlotIndx] = availableTowerSlots[lastAvailableSlotIndex];
        availableTowerSlots[lastAvailableSlotIndex] = -1;
        lastAvailableSlotIndex  -= 1;
        availableSlotCount      -= 1;


        randSlotPosition = allTowerSlots[selectedTowerSlot];
        return true;
    } 

}
