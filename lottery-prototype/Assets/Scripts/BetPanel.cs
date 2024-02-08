using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class BetPanel : MonoBehaviour
{
    private List<PlayerInventory.Items> _betItems = new List<PlayerInventory.Items>();
    private int _numOfBets = 1;
    private Lottery _currentLottery;

    private int _currentItemType = -1;

    public static Action OnRefreshPanel;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // when open panel
    public void SetCurrentLottery(Lottery currentLottery)
    {
        ClearBet();
        _currentLottery = currentLottery;
        OnRefreshPanel();
    }

    public void ShowText(TMP_Text text)
    {
        text.text = $"{CheckItemCount(_currentItemType)}";
    }

    // press raise button
    public void AddBet(int itemType)
    {
        _currentItemType = itemType;

        int currentTypeItemNum = CheckItemCount(itemType);

        if ((currentTypeItemNum + 1) * _numOfBets > PlayerInventory.currentItems[(PlayerInventory.Items)itemType]) return;

        // if (currentTypeItemNum == 0)
        //     if (_numOfBets > PlayerInventory.currentItems[(PlayerInventory.Items)itemType]) return;
        // if fewer than the stock
        _betItems.Add((PlayerInventory.Items)itemType);
    }

    // press remove button
    public void RemoveBet(int itemType)
    {
        _currentItemType = itemType;
        _betItems.Remove((PlayerInventory.Items)itemType);
    }

    // press raise bet number button
    public void RaiseNumOfBets(TMP_Text text)
    {
        for (int i = 0; i < Enum.GetValues(typeof(PlayerInventory.Items)).Length; i++)
        {
            if (CheckItemCount(i) * (_numOfBets + 1) > PlayerInventory.currentItems[(PlayerInventory.Items)i]) return;
        }

        _numOfBets++;
        text.text = "Number Of Bets: " + _numOfBets;
    }

    public void ReduceNumOfBets(TMP_Text text)
    {
        if (_numOfBets <= 1) return;
        _numOfBets--;
        text.text = "Number Of Bets: " + _numOfBets;
    }

    // press submit button
    public void SubmitBets()
    {
        // reduce
        _currentLottery.SetBetInfo(_betItems, _numOfBets);
    }

    private void ClearBet()
    {
        _betItems.Clear();
        _numOfBets = 1;
    }

    private int CheckItemCount(int itemType)
    {
        int count = _betItems.Count(x => x == (PlayerInventory.Items)itemType);
        return count;
    }
}
