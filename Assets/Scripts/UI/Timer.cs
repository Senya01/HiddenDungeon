using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField]
    public int time;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    [HideInInspector] private float timeLeft;
    [HideInInspector] private int secondsLeft;

    private void Start()
    {
        SetMaxValue(time);
        RestartTime();
    }

    private void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            SetTime(secondsLeft);
        }
        else
        {
            if (secondsLeft <= 0)
            {
                foreach (EnemyBase enemyBase in FindObjectsOfType<EnemyBase>())
                {
                    enemyBase.SpawnEnemy();
                }

                RestartTime();
            }

            timeLeft = 1;
            secondsLeft--;
        }
    }

    private void RestartTime()
    {
        SetTime(time);
        timeLeft = 1;
        secondsLeft = time;
    }

    public void SetMaxValue(int value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void SetTime(int seconds)
    {
        slider.value = seconds;
        TimeSpan time = TimeSpan.FromSeconds(seconds);

        text.text = time.ToString(@"mm\:ss");
    }
}