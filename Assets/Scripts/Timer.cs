using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float countdownTime;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject grid;
    private bool _isTimerOn;
    public static Timer Instance { get; private set; }

    void Awake()
    {
        // use singleton to access Timer from GridHandler
        Instance = this;
    }
    
    public void StartTheTimer()
    {
        _isTimerOn = true;
    }

    void Update()
    {
        if (_isTimerOn)
        {
            if (countdownTime > 0)
            {
                countdownTime -= Time.deltaTime;
                // countdownTime++;

                timerText.text = Mathf.RoundToInt(countdownTime).ToString();
            }
            else
            {
                countdownTime = 0;
                _isTimerOn = false;
                GridHandler.Instance.SetTheScore();
                grid.SetActive(false);
                scorePanel.SetActive(true);
            }
        }
    }
}
