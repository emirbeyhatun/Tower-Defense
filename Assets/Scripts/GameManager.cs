using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int killCount = 0;
    public int KillCount { get => killCount; }

    private int gold = 80;
    public int Gold { get => gold; }

    private int towerSpawnPrice = 25;
    public int TowerSpawnPrice { get => towerSpawnPrice; }

    public int towerPriceIncreaseAmount = 55;

    private int killCountLimit = 3000;
    public int KillCountLimit { get => killCountLimit; }

    public bool isGameResuming = false;

    public Dictionary<int, TowerBrainBase> instantiatedTowerBrains = new Dictionary<int, TowerBrainBase>();
    public EnemyFactory enemyFactory;
    public TowerFactory towerFactory;
    public UpgradePanel upgPanel;

    private List<UpdateableGameObject> updateableObjects = new List<UpdateableGameObject>();
    private WaitForSeconds explotionPoolingTime = new WaitForSeconds(2);


    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (isGameResuming)
        {
            RefreshUpdateables();
        }
    }

    private void RefreshUpdateables()
    {
        if (updateableObjects == null)
            return;

        int i = 0;
        while(i < updateableObjects.Count)
        {
            UpdateableGameObject updateable = updateableObjects[i];
            if (updateable != null)
            {
                updateable.UpdateEveryFrame();
            }
            else
            {
                updateableObjects.RemoveAt(i);
                i--;
            }
            i++;
        }
    }
    public void AddUpdateableObject(UpdateableGameObject updateable)
    {
        if (updateable == null || updateableObjects == null)
            return;

        updateableObjects.Add(updateable);
    }

    public void PoolExplotion(TowerBrainBase towerScriptableObj, GameObject explotion)
    {
        StartCoroutine(PoolExplotionViaTimer(towerScriptableObj, explotion));
    }
    IEnumerator PoolExplotionViaTimer(TowerBrainBase towerScriptableObj, GameObject explotion)
    {
        yield return explotionPoolingTime;

        if(towerScriptableObj && explotion)
        {
            explotion.SetActive(false);
            towerScriptableObj.PoolExplotion(explotion);
        }
    }

    public void IncreaseKillCount()
    {
        killCount++;
        UiManager.instance.UpdateKillText();

        if (IsGameWinConditionMet())
        {
            UiManager.instance.OnLevelWon();
        }
    }

    public bool IsGameWinConditionMet()
    {
        if(killCount >= killCountLimit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryToPurchase(int price)
    {
        if(price <= gold)
        {
            gold -= price;

            UpdateSpawnButton();
            UpdateUpgPanel();
            UiManager.instance.UpdateGoldText();
            return true;
        }

        return false;
    }
    public bool CanBePurchased(int price)
    {
        if (price <= gold)
        {
            return true;
        }

        return false;
    }

    public void BuyTower()
    {
        if (towerFactory)
        {
            if (TryToPurchase(towerSpawnPrice))
            {
                towerSpawnPrice += towerPriceIncreaseAmount;

                towerFactory.SpawnTowerOnRandSlot();

                UiManager.instance.UpdateSpawnPanel();
                UiManager.instance.UpdateGoldText();
                UpdateSpawnButton();
                UpdateUpgPanel();
            }
        }
    }

    public void EarnMoney(int amount)
    {
        gold += amount;
        UpdateSpawnButton();
        UpdateUpgPanel();
        UiManager.instance.UpdateGoldText();
    }

    public void UpdateSpawnButton()
    {
        if (towerFactory == null)
            return;

        if (gold >= towerSpawnPrice && towerFactory.GetAvailableSlotAmount() > 0 )
        {
            UiManager.instance.EnableSpawnButton(true);
        }
        else
        {
            UiManager.instance.EnableSpawnButton(false);

        }
    }

    public void UpdateUpgPanel()
    {
        if (upgPanel == null)
            return;

        upgPanel.UpdatePanel();
    }


    public TowerBrainBase TryToGetBrain(TowerBrainBase brainForRef)
    {
        if (brainForRef == null)
            return null;

        TowerBrainBase brain = null;
        if(instantiatedTowerBrains.TryGetValue(brainForRef.ID, out brain))
        {
            return brain;
        }
        else
        {
            brain = Instantiate(brainForRef);
            instantiatedTowerBrains.Add(brain.ID, brain);
            return brain;
        }
    }

}
