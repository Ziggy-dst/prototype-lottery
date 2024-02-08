using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RefreshableText : MonoBehaviour
{
    public string defaultText;
    private TMP_Text text;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        BetPanel.OnRefreshPanel += Refresh;
    }

    private void OnDisable()
    {
        BetPanel.OnRefreshPanel -= Refresh;
    }

    private void Refresh()
    {
        text.text = defaultText;
    }
}
