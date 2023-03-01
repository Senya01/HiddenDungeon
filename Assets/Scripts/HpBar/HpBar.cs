using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private Transform healthBar;
    [SerializeField] private GameObject healthBarContainer;
    [SerializeField] private float hideTime;

    [HideInInspector] public float maxValue;
    [HideInInspector] private float value;

    [HideInInspector] private float hideTimeLeft;
    [HideInInspector] private bool hideTimer;

    private void Start()
    {
        HideBar(true);
    }

    private void Update()
    {
        HideTimer();
    }

    private void HideBar(bool hide)
    {
        healthBarContainer.SetActive(!hide);
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
                HideBar(true);
                hideTimer = false;
            }
        }
    }

    public void UpdateBar(float hp)
    {
        value = hp;
        healthBar.localScale = new Vector3(value / maxValue, 1, 1);
        hideTimeLeft = hideTime;
        hideTimer = true;
        HideBar(false);
    }
}