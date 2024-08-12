using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreTextTMP;

    private void Awake()
    {
        scoreTextTMP = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnScoreChanged += StaticEventHandler_OnScoreChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnScoreChanged -= StaticEventHandler_OnScoreChanged;
    }

    private void StaticEventHandler_OnScoreChanged(ScoreChangedArgs scoreChangedArgs)
    {
        scoreTextTMP.text = "SCORE: " + scoreChangedArgs.score.ToString("###,###0"); 
    }
}
