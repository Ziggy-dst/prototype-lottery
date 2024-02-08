using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // player inventory
    public List<TMP_Text> inventoryUIList;

    // date
    public TMP_Text weekCountUI;
    public TMP_Text dateUI;

    // goal
    public List<TMP_Text> resolveGoalUIList;
    public TMP_Text resolveDateUI;

    // game state
    public GameObject losePanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public void ChangeWeekCountUI(int newText)
    {
        weekCountUI.text = "Week " + newText.ToString();
    }

    public void ChangeDateUI(Calendar.Day newText)
    {
        dateUI.text = newText.ToString();
    }

    public void ChangeResolveDateUI(int newText)
    {
        resolveDateUI.text = "Next Resolve Date: " + newText;
    }

    public void ChangeInventoryUI(Dictionary<PlayerInventory.Items, int> newText)
    {
        foreach (var i in inventoryUIList)
        {
            i.text = newText[(PlayerInventory.Items)inventoryUIList.IndexOf(i)].ToString();
        }
    }

    public void ChangeResolveGoalUI(Dictionary<PlayerInventory.Items, int> newText)
    {
        foreach (var i in resolveGoalUIList)
        {
            i.text = newText[(PlayerInventory.Items)resolveGoalUIList.IndexOf(i)].ToString();
        }
    }
}
