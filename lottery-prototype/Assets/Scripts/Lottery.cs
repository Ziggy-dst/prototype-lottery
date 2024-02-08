using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

[Serializable]
public class RewardAttribute
{
    public PlayerInventory.Items item;
    public int itemCount;
}
// TODO: 限制投注道具总量和下注数量
public class Lottery : SerializedMonoBehaviour
{
    [Title("Prize Settings")]
    // reward related
    public int prizeThreshold = 2;
    public int prizeModifier = 1;
    public Calendar.Day prizeDay = Calendar.Day.Mon;
    [DictionaryDrawerSettings(KeyLabel = "Item Name", ValueLabel = "Count")]
    public Dictionary<PlayerInventory.Items, int> BasePrize = new Dictionary<PlayerInventory.Items, int>();

    [Title("Lottery Draw Settings")]
    // result resolve related
    [PropertyTooltip("What kinds of item might appear in the result")]
    public List<PlayerInventory.Items> possibleResultTypes;
    [PropertyTooltip("How many items the result would have")]
    public int resultNum = 3;

    [Title("UI")]
    public TMP_Text prizeDayUI;
    public List<TMP_Text> basePrizeUI;
    public TMP_Text possibleResultTypesUI;
    public TMP_Text resultNumUI;
    public List<TMP_Text> betPlacedUIList;

    // player's choice
    private List<PlayerInventory.Items> _itemsPaid = new List<PlayerInventory.Items>();
    private int numberOfBets = 0;
    public static Action<Dictionary<PlayerInventory.Items, int>> OnStockChanged;

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        prizeDayUI.text = "Date: " + prizeDay;
        possibleResultTypesUI.text = "Possible Results: " + string.Join(", ", possibleResultTypes);
        resultNumUI.text = "Num of result items: " + resultNum;

        ChangePrizeUIList(basePrizeUI, BasePrize);
    }

    private void ChangePrizeUIList(List<TMP_Text> UIList, Dictionary<PlayerInventory.Items, int> newText)
    {
        foreach (var i in UIList)
        {
            var currentKey = (PlayerInventory.Items)UIList.IndexOf(i);

            if (!BasePrize.ContainsKey(currentKey)) continue;
            i.text = (int.Parse(i.text) + newText[currentKey]).ToString();
        }
    }

    private void ChangePlacedBetUIList(List<TMP_Text> UIList, Dictionary<PlayerInventory.Items, int> newText)
    {
        foreach (var i in UIList)
        {
            var currentKey = (PlayerInventory.Items)UIList.IndexOf(i);
            i.text = (int.Parse(i.text) + Math.Abs(newText[currentKey])).ToString();
        }
    }

    private void OnEnable()
    {
        Calendar.OnLotteryDraw += GenerateResults;
    }

    private void OnDisable()
    {
        Calendar.OnLotteryDraw -= GenerateResults;
    }

    public void SetBetInfo(List<PlayerInventory.Items> _betItems, int numOfBets)
    {
        _itemsPaid.AddRange(_betItems);
        numberOfBets = numOfBets;

        Dictionary<PlayerInventory.Items, int> itemToReduce = new Dictionary<PlayerInventory.Items, int>();
        for (int i = 0; i < Enum.GetValues(typeof(PlayerInventory.Items)).Length; i++)
        {
            itemToReduce.Add((PlayerInventory.Items)i, 0);
        }

        foreach (var item in _betItems)
        {
            if (itemToReduce.ContainsKey(item))
            {
                itemToReduce[item] -= numOfBets;
            }
        }

        ChangePlacedBetUIList(betPlacedUIList, itemToReduce);

        // reduce from the inventory
        OnStockChanged(itemToReduce);
    }

    private void GenerateResults(Calendar.Day currentDay)
    {
        if (!currentDay.Equals(prizeDay)) return;

        transform.DOShakeScale(.2f, new Vector3(1.001f, 1.001f, 1f), 0, 0);

        List<PlayerInventory.Items> results = new List<PlayerInventory.Items>();
        for (int i = 0; i < resultNum; i++)
        {
            results.Add(possibleResultTypes[Random.Range(0,possibleResultTypes.Count)]);
            print("result: " + possibleResultTypes[Random.Range(0, possibleResultTypes.Count)]);
        }

        CompareResults(results);
    }

    private void CompareResults(List<PlayerInventory.Items> results)
    {
        int sameElementsCount = _itemsPaid.Intersect(results).Count();
        _itemsPaid.Clear();

        Reward(sameElementsCount);
    }


    private void Reward(int sameElementsCount)
    {
        if (sameElementsCount < prizeThreshold) return;

        int rewardMultiplier = prizeModifier * sameElementsCount * numberOfBets;

        Dictionary<PlayerInventory.Items, int> playerRewards = new Dictionary<PlayerInventory.Items, int>();
        foreach (var reward in BasePrize)
        {
            // Dictionary<PlayerInventory.Items, int> currentReward = new Dictionary<PlayerInventory.Items, int>();
            // currentReward.Add(reward.Key, reward.Value * rewardMultiplier);
            // currentReward.item = reward.item;
            // currentReward.itemCount = reward.itemCount * rewardMultiplier;
            playerRewards.Add(reward.Key, reward.Value * rewardMultiplier);
        }

        OnStockChanged(playerRewards);
    }
}
