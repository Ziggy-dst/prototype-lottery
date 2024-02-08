using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: 结算周期随着游戏推进加长
public class Calendar : MonoBehaviour
{
    // date info
    // mon, tue, wed...
    private Day _currentDay = Day.Mon;
    private int _weekCount = 0;

    // resolve date related
    private int _resolvePassed = 0;
    private int _nextResolutionDate = 10;
    public int resolvePeriod = 10;
    public int baseGoalItemNum = 5;
    public int goalItemNumIncrement = 2;
    private Dictionary<PlayerInventory.Items, int> _resolveGoal = new Dictionary<PlayerInventory.Items, int>();

    public static Action<Dictionary<PlayerInventory.Items, int>> OnResolve;
    public static Action<Day> OnLotteryDraw;

    public enum Day
    {
        Mon = 1,
        Tue = 2,
        Wed = 3,
        Thu = 4,
        Fri = 5,
        Sat = 6,
        Sun = 7
    }

    void Start()
    {
        print("----------------------Game Start-----------------------");
        _resolvePassed = 0;
        _nextResolutionDate = resolvePeriod;
        _currentDay = (Day)Random.Range(1, 7);

        UIManager.Instance.ChangeDateUI(_currentDay);
        UIManager.Instance.ChangeWeekCountUI(_weekCount);
        UIManager.Instance.ChangeResolveDateUI(_nextResolutionDate);

        InitializeResolveGoal();
        GenerateResolveGoal();
        // print("Day: " + _dayCount);
        // print("Next Resolve Date: " + _nextResolutionDate);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextDay();
        }
    }

    private void InitializeResolveGoal()
    {
        for (int i = 0; i < Enum.GetValues(typeof(PlayerInventory.Items)).Length; i++)
        {
            _resolveGoal.Add((PlayerInventory.Items)i, 0);
        }
    }

    private void NextDay()
    {
        if (_currentDay == Day.Sun)
        {
            _currentDay = Day.Mon;
            _weekCount++;
        }
        else _currentDay++;

        _nextResolutionDate--;

        if (_nextResolutionDate == 0)
        {
            OnResolve(_resolveGoal);

            // reset the date counter
            _nextResolutionDate = resolvePeriod;
            _resolvePassed++;
            GenerateResolveGoal();
        }

        // check if each lottery should draw today
        OnLotteryDraw(_currentDay);

        UIManager.Instance.ChangeDateUI(_currentDay);
        UIManager.Instance.ChangeWeekCountUI(_weekCount);
        UIManager.Instance.ChangeResolveDateUI(_nextResolutionDate);

        // print("Week: " + _weekCount);
        // print("Day: " + _dayCount + " " + _currentDay);
        // print("Next Resolve Date: " + _nextResolutionDate);
    }

    private void GenerateResolveGoal()
    {
        // goal will be more difficult as resolve times goes up
        // _resolveGoal.Clear();

        int nextGoalItemNum = baseGoalItemNum + goalItemNumIncrement * _resolvePassed;

        for (int i = 0; i < nextGoalItemNum; i++)
        {
            PlayerInventory.Items goalItem = (PlayerInventory.Items)Random.Range(0, Enum.GetValues(typeof(PlayerInventory.Items)).Length);
            _resolveGoal[goalItem]++;
        }

        UIManager.Instance.ChangeResolveGoalUI(_resolveGoal);
    }
}