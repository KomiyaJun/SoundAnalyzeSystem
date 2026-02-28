using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BannerController : MonoBehaviour
{
    [Header("References")]
    public GameObject bannerImagePrefab;
    public RectTransform content;
    public RectTransform bannerMainPanel;
    public InputReader _inputReader;

    [Header("Dot Settings")]
    public GameObject dotPrefab;
    public Transform dotParent;

    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;
    public TextMeshProUGUI nextButtonText;

    [Header("Animation Settings")]
    public float bannerWidth = 800f;
    public float animationDuration = 0.5f;
    public float startPositionY = 1000f;
    public float minScale = 0.8f;

    private int currentIndex = 0;
    private List<Image> dotImages = new List<Image>();
    private int bannerCount = 0;

    public bool IsClosed { get; private set; } = false;

    private void OnEnable()
    {
        _inputReader.NextPageEvent += OnClickRight;
        _inputReader.BackPageEvent += OnClickLeft;
    }

    private void OnDisable()
    {
        _inputReader.NextPageEvent -= OnClickRight;
        _inputReader.BackPageEvent -= OnClickLeft;

    }


    // Start時の初期化をSetupUIに統合
    void Start()
    {
        if (content.childCount > 0)
        {
            SetupUI();
        }
    }

    public void OnClickRight()
    {
        if (currentIndex < bannerCount - 1)
        {
            currentIndex++;
            UpdateUI();
        }
        else
        {
            CloseBanner();
        }
    }

    public void OnClickLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }

    void UpdateUI(bool immediate = false)
    {
        float targetPos = -currentIndex * bannerWidth;

        if (immediate)
        {
            content.DOKill();
            content.anchoredPosition = new Vector2(targetPos, content.anchoredPosition.y);
        }
        else
        {
            content.DOAnchorPosX(targetPos, 0.3f).SetEase(Ease.OutQuint);
        }

        if (leftButton != null) leftButton.interactable = (currentIndex > 0);

        if (nextButtonText != null)
        {
            nextButtonText.text = (currentIndex == bannerCount - 1) ? "とじる" : "つぎへ";
        }

        for (int i = 0; i < dotImages.Count; i++)
        {
            if (dotImages[i] == null) continue;
            dotImages[i].color = (i == currentIndex) ? Color.white : new Color(1, 1, 1, 0.3f);
        }
    }

    public void OpenBanner()
    {
        IsClosed = false;
        gameObject.SetActive(true);

        // 開く前に座標をリセット
        currentIndex = 0;
        UpdateUI(true);

        bannerMainPanel.DOKill();
        bannerMainPanel.anchoredPosition = new Vector2(0, startPositionY);
        bannerMainPanel.localScale = Vector3.one * minScale;

        Sequence openSequence = DOTween.Sequence();
        openSequence
            .Append(bannerMainPanel.DOAnchorPosY(0, animationDuration).SetEase(Ease.OutQuad))
            .Append(bannerMainPanel.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
    }

    public void CloseBanner()
    {
        bannerMainPanel.DOKill();
        bannerMainPanel.DOScale(minScale, animationDuration * 0.5f).SetEase(Ease.InBack);
        bannerMainPanel.DOAnchorPosY(startPositionY, animationDuration)
            .SetEase(Ease.InBack)
            .SetDelay(0.1f)
            .OnComplete(() =>
            {
                IsClosed = true;
                gameObject.SetActive(false);
            });

        _inputReader.EnablePlayerEvent();
    }

    public void OpenWithData(BannerData data)
    {
        // --- 修正箇所：逆順ループで確実に全て削除する ---
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Transform child = content.GetChild(i);
            child.SetParent(null); // 親子関係を即座に切る
            Destroy(child.gameObject);
        }

        foreach (var sprite in data.bannerSprites)
        {
            GameObject obj = Instantiate(bannerImagePrefab, content);
            obj.GetComponent<Image>().sprite = sprite;
        }

        SetupUI();
        OpenBanner();

        _inputReader.EnableUIEvent();
    }

    private void SetupUI()
    {
        bannerCount = content.childCount;

        // --- 修正箇所：ドットも同様に安全に削除 ---
        for (int i = dotParent.childCount - 1; i >= 0; i--)
        {
            Transform child = dotParent.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        dotImages.Clear();

        for (int i = 0; i < bannerCount; i++)
        {
            GameObject dotObj = Instantiate(dotPrefab, dotParent);
            Image dotImg = dotObj.GetComponent<Image>();
            if (dotImg != null) dotImages.Add(dotImg);
        }

        // (以下略)
        if (nextButtonText == null && rightButton != null)
        {
            nextButtonText = rightButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        currentIndex = 0;
        UpdateUI(true);
    }
}