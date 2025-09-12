using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField] private Image currentHeadImage;
    [SerializeField] private Sprite[] hpHeads;
    [SerializeField] private TextMeshProUGUI hpText;

    private int maxHealth;
    private int segments;
    private int healthPerSegment;

    public void Initialize(int maxHealth)
    {
        this.maxHealth = maxHealth;
        segments = hpHeads.Length;
        healthPerSegment = Mathf.CeilToInt((float)maxHealth / segments);

        UpdateHeadSprite(maxHealth);
        UpdateHpText(maxHealth);
    }

    public void UpdateUI(int currentHealth)
    {
        UpdateHeadSprite(currentHealth);
        UpdateHpText(currentHealth);
    }

    public void UpdateHeadSprite(int currentHealth)
    {
        int spriteIndex = Mathf.Clamp(segments - 1 - (currentHealth / healthPerSegment), 0,
            hpHeads.Length - 1);
        currentHeadImage.sprite = hpHeads[spriteIndex];
    }
    private void UpdateHpText(int currentHealth)
    {
        hpText.text = $"{currentHealth}";
    }
}
