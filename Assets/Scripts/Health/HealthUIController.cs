using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class HealthUIController : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Health health;
    [SerializeField] private Sprite fullHeartSprite;    //体力画像
    [SerializeField] private Sprite emptyHeartSprite;   //ダメージ後の体力画像

    [Header("ハートのリスト")]
    [SerializeField] private List<Image> heartImages = new List<Image>();


    private void OnEnable()
    {
        health.OnHealthChanged += UpdateUI;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= UpdateUI;
    }

    private void UpdateUI(int currentHealth)
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if(i < currentHealth)
            {
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite;
            }
        }
        
    }
}
