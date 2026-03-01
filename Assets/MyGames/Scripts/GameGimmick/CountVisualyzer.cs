using UnityEngine;
using System.Collections.Generic;

public class CountVisualyzer : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameObject iconPrefab;

    [Header("配置")]
    [SerializeField] private Vector3 startOffset = new Vector3(-1.5f, 2.0f, 0f);
    [SerializeField] private Vector3 spacing = new Vector3(1.0f, 0f, 0f);

    [Header("画像素材")]
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    [Header("色")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color maxColor = Color.red;

    private List<SpriteRenderer> _spawnedIcons = new List<SpriteRenderer>();



    public void InitializeVisualyze(int maxCount)
    {
        foreach(var icon in _spawnedIcons)
        {
            if (icon != null)
            {
                Destroy(icon.gameObject);
            }
        }
        _spawnedIcons.Clear();

        for(int i = 0;  i < maxCount; i++)
        {
            GameObject obj = Instantiate(iconPrefab, transform);
            obj.transform.localPosition = startOffset + (spacing * i);
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if(sr != null)
            {
                sr.sprite = inactiveSprite;
                _spawnedIcons.Add(sr);
            }
        }

    }

    public void UpdateVisuals(int currentCount)
    {
        if (_spawnedIcons.Count == 0) return;

        for(int i = 0; i < _spawnedIcons.Count; i++)
        {
            if (i < currentCount)
            {
                _spawnedIcons[i].color = normalColor;
                _spawnedIcons[i].sprite = activeSprite;
            }
            else
            {
                _spawnedIcons[i].sprite = inactiveSprite;
            }
        }
    }

    public void MaxVisual()
    {
        StartCoroutine(ChangeColor());
    }


    private System.Collections.IEnumerator ChangeColor()
    {
        foreach(var icon in _spawnedIcons)
        {
            if (icon != null) icon.color = maxColor;
        }

        yield return new WaitForSeconds(0.15f);

    }

}
