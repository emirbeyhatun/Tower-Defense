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


    public bool isGameResuming = false;

    public EnemyFactory enemyFactory;
    public TowerFactory towerFactory;
    public UpgradePanel upgPanel;



    private static readonly int updateablesStartSize = 100;
    private UpdateableGameObject[] updateableObjects = new UpdateableGameObject[updateablesStartSize];
    private int updateableCurrentIndex = 0;
    private WaitForSeconds explotionPoolingTime = new WaitForSeconds(2);

    public Dictionary<int, TowerBrainBase> instantiatedTowerBrains = new Dictionary<int, TowerBrainBase>();

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
        while(i < updateableCurrentIndex)
        {
            UpdateableGameObject updateable = updateableObjects[i];
            if (updateable != null)
            {
                updateable.UpdateEveryFrame();
            }
            else
            {
                if (i < updateableCurrentIndex - 1)
                {
                    updateableObjects[i] = updateableObjects[updateableCurrentIndex - 1];
                    updateableCurrentIndex--;
                }
                else if (i == updateableCurrentIndex - 1)
                {
                    updateableCurrentIndex--;
                }
            }
            i++;
        }
    }
    public void AddUpdateableObject(UpdateableGameObject updateable)
    {
        if (updateable == null)
            return;

        if (updateableCurrentIndex >= updateableObjects.Length)
        {
            UpdateableGameObject[] newUpdArray = new UpdateableGameObject[updateableObjects.Length * 2];

            updateableObjects.CopyTo(newUpdArray, 0);
            updateableObjects = newUpdArray;
        }

        updateableObjects[updateableCurrentIndex] = updateable;
        updateableCurrentIndex++;
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

        if (gold >= towerSpawnPrice && towerFactory.GetAvailableTowerSlotAmount() > 0 )
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
