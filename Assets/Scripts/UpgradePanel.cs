using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    public Text killCountText;
    public Text upgPriceText;
    public Text upgBtnText;
    public Text upgBtnNotEnoughText;
    public Text upgBtnNoMoreUpgText;

    public Button upgBtn;
    public Tower tower;

    public void Upgrade()
    {
        if (tower == null)
            return;
        if (tower.brain == null)
            return;
        if (tower.brain.upgradedVersion == null)
            return;

        if (GameManager.instance.TryToPurchase(tower.brain.upgradedVersion.price))
        {
            tower.SetBrain(tower.brain.upgradedVersion);
            UpdatePanel();
        }
       // gameObject.SetActive(false);
    }

    public void UpdateUpgradeBtn()
    {
        if (tower == null)
            return;
        if (tower.brain == null)
            return;

        if (upgBtn)
        {
            if (tower.brain.upgradedVersion != null)
            {
                if (GameManager.instance.CanBePurchased(tower.brain.upgradedVersion.price)  && tower.killedEnemy >= tower.brain.upgradedVersion.killLimitForUpgrade)
                {
                    upgBtn.interactable = true;
                    if (upgBtnText)
                        upgBtnText.gameObject.SetActive(true);
                    if (upgBtnNotEnoughText)
                        upgBtnNotEnoughText.gameObject.SetActive(false);

                    if (upgBtnNoMoreUpgText)
                        upgBtnNoMoreUpgText.gameObject.SetActive(false);
                }
                else
                {
                    upgBtn.interactable = false;

                    if (upgBtnText)
                        upgBtnText.gameObject.SetActive(false);
                    if (upgBtnNoMoreUpgText)
                        upgBtnNoMoreUpgText.gameObject.SetActive(false);

                    if (upgBtnNotEnoughText)
                    {
                        upgBtnNotEnoughText.gameObject.SetActive(true);
                        if(GameManager.instance.CanBePurchased(tower.brain.upgradedVersion.price) == false)
                        {
                            upgBtnNotEnoughText.text = "Not Enough Money";
                        }
                        else 
                        {
                            upgBtnNotEnoughText.text = "Not Enough Kill";
                        }
                    }
                    tower.ResetWarning();
                }
            }
            else
            {
                upgBtn.interactable = false;

                if (upgBtnText)
                    upgBtnText.gameObject.SetActive(false);
                if (upgBtnNotEnoughText)
                    upgBtnNotEnoughText.gameObject.SetActive(false);

                if (upgBtnNoMoreUpgText)
                    upgBtnNoMoreUpgText.gameObject.SetActive(true);

                tower.ResetWarning();
            }
        }
    }

    public void UpdateText()
    {
        if (tower == null)
            return;
        if (tower.brain == null)
            return;

        if (killCountText && upgPriceText)
        {
            if (tower.brain.upgradedVersion != null)
            {
                killCountText.text = "Kill Count: " + tower.killedEnemy +"/" + tower.brain.upgradedVersion.killLimitForUpgrade;
                upgPriceText.text = tower.brain.upgradedVersion.price.ToString();
            }
            else
            {
                killCountText.text = "Kill Count: " + tower.killedEnemy;
                upgPriceText.text = "--";
            }
        }
    }

    public void PreparePanel(Tower newTower)
    {
        tower = newTower;
        UpdatePanel();
        gameObject.SetActive(true);
    }

    public void UpdatePanel()
    {
        UpdateText();
        UpdateUpgradeBtn();
    }
}
