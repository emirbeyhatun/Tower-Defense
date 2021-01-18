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
    public LayerMask towerLayermask;

    private List<Tower> spawnedTowers;
    private List<Vector3Int> availableSlots;


     
    private void Awake()
    {
        PrepareFactory();
    }

    private void Start()
    {
        GameManager.instance.AddUpdateableObject(this);
    }

    private void PrepareFactory()
    {
        availableSlots = new List<Vector3Int>();

        Vector3Int tilemapOrigin = towerSlotTilemap.origin;

        for (int i = tilemapOrigin.x; i < tilemapOrigin.x + towerSlotTilemap.size.x; i++)
        {
            for (int j = tilemapOrigin.y; j < tilemapOrigin.y + towerSlotTilemap.size.y; j++)
            {
                Vector3Int tileIndices = new Vector3Int(i, j, 0);
                if ((Tile)towerSlotTilemap.GetTile(tileIndices))
                {
                    availableSlots.Add(tileIndices);
                }
            }
        }
        spawnedTowers = new List<Tower>();
    }

    public override void UpdateEveryFrame()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.instance.upgPanel.gameObject.activeSelf == false)
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = towerSlotTilemap.WorldToCell(worldPos);

            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, towerLayermask);
            if (hitInfo.collider)
            {
                Tower collidedTower = hitInfo.collider.GetComponent<Tower>();
                if (collidedTower)
                {
                    GameManager.instance.upgPanel.PreparePanel(collidedTower);
                }
            }
        }
    }

    public override void UpdateFixedFrame()
    {
        throw new NotImplementedException();
    }

    public bool SpawnTowerOnRandSlot()
    {
        if (towerSlotTilemap == null|| availableSlots == null || towerPrefab == null)
            return false;

        Vector3Int randSlotPosition;
        if (TryToPopRandAvailableSlot(out randSlotPosition))
        {
            Vector3 slotWorldPos = towerSlotTilemap.GetCellCenterWorld(randSlotPosition);
            SpawnTowerAtPos(slotWorldPos);
            return true;
        }
        return false;
    }
    private bool TryToPopRandAvailableSlot(out Vector3Int randSlotPosition)
    {
        if (availableSlots == null)
        {
            randSlotPosition = Vector3Int.zero;
            return false;
        }
        if (availableSlots.Count <= 0)
        {
            randSlotPosition = Vector3Int.zero;
            return false;
        }

        int randSlotIndx = UnityEngine.Random.Range(0, availableSlots.Count);
        randSlotPosition = availableSlots[randSlotIndx];
        availableSlots.RemoveAt(randSlotIndx);

        return true;
    }

    private void SpawnTowerAtPos(Vector3 pos)
    {
        if (towerPrefab == null || spawnedTowers == null)
            return;
        
        Tower newTower = Instantiate(towerPrefab, pos, Quaternion.identity, null).GetComponent<Tower>();
        if (newTower)
        {
            spawnedTowers.Add(newTower);
            GameManager.instance.AddUpdateableObject(newTower);
        }
    }

    public int GetSpawnedAmount()
    {
        if (spawnedTowers == null)
        {
            return 0;
        }
        if (spawnedTowers.Count <= 0)
        {
            return 0;
        }
        return spawnedTowers.Count;
    }

    public int GetAvailableSlotAmount()
    {
        if (availableSlots == null)
        {
            return 0;
        }
        if (availableSlots.Count <= 0)
        {
            return 0;
        }

        return availableSlots.Count;
    }

    
}
