using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    public GameObject EndGameUI;
    public GameObject ReadyGameUI;

    public Button towerSpawnButton;
    public Text spawnedTowerAmountText;
    public Text towerSpawnPriceText;
    public Text goldText;
    public Text killText;



    private void Awake()
    {
        instance = this;

        OnLevelReady();
    }

    private void Start()
    {
        UpdateSpawnPanel();
        UpdateGoldText();
    }

    public void UpdateSpawnPanel()
    {
        if (towerSpawnPriceText)
        {
            towerSpawnPriceText.text = "Price: " + GameManager.instance.TowerSpawnPrice.ToString();
        }

        if (spawnedTowerAmountText && GameManager.instance.towerFactory)
        {
            int spawnedAmount = GameManager.instance.towerFactory.GetSpawnedTowerAmount();
            spawnedTowerAmountText.text = spawnedAmount + " / " + (spawnedAmount + GameManager.instance.towerFactory.GetAvailableTowerSlotAmount()) ;
        }
    }

    public void UpdateGoldText()
    {
        if (goldText)
        {
            goldText.text = GameManager.instance.Gold.ToString();
        }
    }

    public void UpdateKillText()
    {
        if (killText)
        {
            killText.text = GameManager.instance.KillCount.ToString();
        }
    }

    public void EnableSpawnButton(bool enableBtn)
    {
        if (towerSpawnButton)
        {
            towerSpawnButton.interactable = enableBtn;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);

        GameManager.instance = null;

    }

    public void OnLevelLost()
    {
        GameManager.instance.isGameResuming = false; 
        Time.timeScale = 0;
        if (EndGameUI)
        {
            EndGameUI.gameObject.SetActive(true);
        }

        if (towerSpawnButton)
        {
            towerSpawnButton.gameObject.SetActive(false);
        }
    }

    public void OnLevelReady()
    {
        Time.timeScale = 0;
        if (ReadyGameUI)
            ReadyGameUI.SetActive(true);

        if (towerSpawnButton)
        {
            towerSpawnButton.gameObject.SetActive(false);
        }
    }
    public void OnLevelStart()
    {
        Time.timeScale = 1;
        if (ReadyGameUI)
            ReadyGameUI.SetActive(false);

        GameManager.instance.isGameResuming = true;

        if (towerSpawnButton)
        {
            towerSpawnButton.gameObject.SetActive(true);
        }
    }

}
