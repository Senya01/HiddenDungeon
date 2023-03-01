using System;
using TMPro;
using UnityEngine;

public class Hints : MonoBehaviour
{
    [Header("Base settings")] [SerializeField]
    private TextMeshProUGUI hintText;

    [SerializeField] private float hideTime;
    [SerializeField] private string[] hintsText;

    [HideInInspector] private float hideTimeLeft;
    [HideInInspector] private bool hideTimer;

    public void ShowHint(int index, object[] format = null)
    {
        if (!hideTimer)
        {
            hideTimer = true;
            hideTimeLeft = hideTime;

            hintText.text = format != null ? string.Format(hintsText[index], format) : hintsText[index];
            hintText.enabled = true;
        }
    }

    private void HideTimer()
    {
        if (hideTimer)
        {
            if (hideTimeLeft > 0)
            {
                hideTimeLeft -= Time.deltaTime;
            }
            else
            {
                hideTimer = false;
                hintText.enabled = false;
            }
        }
    }

    public void Update()
    {
        HideTimer();
    }
}