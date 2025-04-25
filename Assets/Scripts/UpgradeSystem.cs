using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UpgradeSystem : MonoBehaviour
{
    public GameObject upgradeUI;
    public Button[] upgradeButtons;
    private Player player;
    private List<string> allUpgrades = new List<string>();
    private List<string> allAscensions = new List<string>();
    private List<bool> ascensions = new List<bool>();

    private List<bool> upgraded = new List<bool>();

    private void Start()
    {
        player = FindObjectOfType<Player>();
        upgradeUI.SetActive(false);
        InitializeUpgrades();
    }

    private void InitializeUpgrades()
    {
        allUpgrades.Add("Increase Damage. 30% increased Damage."); //0
        allUpgrades.Add("Increase Movement Speed. 50% increased Movement Speed."); //1
        allUpgrades.Add("Piercing Bullets"); //2
        allUpgrades.Add("Explosive Bullets"); //3
        allUpgrades.Add("Rapid Fire. 30% increased Attack Speed."); //4
        allUpgrades.Add("Health Boost. Increase Max Health by 60%."); //5
        allUpgrades.Add("Chaining Bullets"); //6
        allUpgrades.Add("+1 Projectile"); //7
        allUpgrades.Add("+3 Projectiles. -50% Damage"); //8
        allUpgrades.Add("Slow and steady. 2x Damage, -30% Attack Speed."); //9
        allUpgrades.Add("Minigun. 5x Attack Speed. -60% Damage."); //10
        allUpgrades.Add("Herald of Lightning. Lightning Strikes Enemies you Hit for every Third Hit."); //11
        allUpgrades.Add("Herald of Ice. Enemies you Hit have actions slowed by 50%."); //12
        allUpgrades.Add("Herald of Ash. Enemies you Hit take Burning Damage."); //13
        allUpgrades.Add("Massive Bullets. Bullets are 3x larger. 10% increased Damage."); //14
        allUpgrades.Add("Miniaturize. 15% Increased Attack Speed, Movement Speed. 30% reduced Size."); //15
        allUpgrades.Add("Lucky Shot. Bullets have 20% chance to deal Double Damage."); //16
        allUpgrades.Add("Dash. Gain the ability to Dash using Spacebar."); //17
        allUpgrades.Add("Heavy Hitter. 50% increased Damage. -30% Movement Speed."); //18
        allUpgrades.Add("Glass Cannon. 2x Damage. -60% Maximum Health."); //19
        allUpgrades.Add("Jack of all Trades. 10% increased Damage. 10% increased Attack Speed. 10% increased Movement Speed. 10% increased Maximum Health."); //20
        allUpgrades.Add("Increase Health Regeneration to 3 Health per Second"); //21
        allUpgrades.Add("Increase Area of Effect by 60%"); //22
        upgraded = new List<bool>(new bool[allUpgrades.Count]);

        allAscensions.Add("Railgun");
        allAscensions.Add("Grenade Launcher");
        allAscensions.Add("Crystal Machinegun");
        ascensions = new List<bool>(new bool[allAscensions.Count]);
    }

    public void LevelUp()
    {
        Time.timeScale = 0f;
        upgradeUI.SetActive(true);
        ShowUpgrades();
    }

    public void Ascend()
    {
        Time.timeScale = 0f;
        upgradeUI.SetActive(true);
        ShowAscensions();
    }

    private void ShowUpgrades()
    {
        List<int> availableIndexes = new List<int>();
        for (int i = 0; i < allUpgrades.Count; i++)
        {
            if (!upgraded[i]) availableIndexes.Add(i);
        }

        List<int> selectedIndexes = new List<int>();
        while (selectedIndexes.Count < 3 && availableIndexes.Count > 0)
        {
            int index = Random.Range(0, availableIndexes.Count);
            selectedIndexes.Add(availableIndexes[index]);
            availableIndexes.RemoveAt(index);
        }

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < selectedIndexes.Count)
            {
                int upgradeIndex = selectedIndexes[i];
                upgradeButtons[i].gameObject.SetActive(true);
                upgradeButtons[i].GetComponentInChildren<TMP_Text>().text = allUpgrades[upgradeIndex];
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(upgraded, upgradeIndex));
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ShowAscensions()
    {
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < allAscensions.Count)
            {
                int index = i;
                upgradeButtons[i].gameObject.SetActive(true);
                upgradeButtons[i].GetComponentInChildren<TMP_Text>().text = allAscensions[index];
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => ApplyAscension(ascensions, index));
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }

        upgradeUI.SetActive(true);
        Time.timeScale = 0f;
    }


    private void ApplyUpgrade(List<bool> upgraded, int index)
    {
        if (!upgraded[index])
        {
            upgraded[index] = true;
            player.ApplyUpgrade(index);
        }
        upgradeUI.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ApplyAscension(List<bool> ascensions, int index)
    {
        Debug.Log(index);
        if (!ascensions[index])
        {
            ascensions[index] = true;
            player.ApplyAscension(index);
        }
        upgradeUI.SetActive(false);
        Time.timeScale  = 1f;
    }
}
