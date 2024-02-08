using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInventory : MonoBehaviour
{
    public static Dictionary<Items, int> currentItems = new Dictionary<Items, int>();

    public int initialItemNum = 5;

    public enum Items
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4
    }

    void Start()
    {
        currentItems.Clear();
        GenerateInitialItems();
        GenerateInitialItemCounts();
        UIManager.Instance.ChangeInventoryUI(currentItems);
    }

    private void OnEnable()
    {
        Lottery.OnStockChanged += ChangeInventoryStock;
        Calendar.OnResolve += Resolve;
    }

    private void OnDisable()
    {
        Lottery.OnStockChanged -= ChangeInventoryStock;
        Calendar.OnResolve -= Resolve;
    }

    private void GenerateInitialItems()
    {
        int itemTypeCount = Enum.GetValues(typeof(Items)).Length;
        // print(itemTypeCount);
        for (int i = 0; i < itemTypeCount; i++)
        {
            currentItems.Add((Items)i, 0);
        }
    }

    private void GenerateInitialItemCounts()
    {
        for (int i = 0; i < initialItemNum; i++)
        {
            int randItemIndex = Random.Range(0, currentItems.Count);
            currentItems[(Items)randItemIndex]++;
        }
    }

    private void ChangeInventoryStock(Dictionary<Items, int> changes)
    {
        foreach (var pair in changes)
        {
            Items key = pair.Key;

            if (currentItems.ContainsKey(key))
            {
                currentItems[key] += changes[key];
            }
        }

        UIManager.Instance.ChangeInventoryUI(currentItems);
    }

    private void Resolve(Dictionary<Items, int> resolveGoal)
    {
        // check if the player could pass the resolve
        for (int i = 0; i < currentItems.Count; i++)
        {
            if (currentItems[(Items)i] < resolveGoal[(Items)i])
            {
                print("You lose");
                GameManager.Instance.EndGame();
            }
            else
            {
                print("Resolved");
                currentItems[(Items)i] -= resolveGoal[(Items)i];
                UIManager.Instance.ChangeInventoryUI(currentItems);
            }
        }
    }
}
