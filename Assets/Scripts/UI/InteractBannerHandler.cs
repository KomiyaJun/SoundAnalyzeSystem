using UnityEngine;

public class InteractBannerHandler : MonoBehaviour, IInteractable
{
    [Header("参照")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private BannerController _bannerController;

    [Header("表示内容")]
    [SerializeField] private BannerData _bannerData;

    [Header("光る演出")]
    [SerializeField] private SpriteRenderer _targetSprite;
    [SerializeField] private Color _highlightColor = new Color(1f, 1f, 0.7f);

    private Color _originalColor;
    private bool _isPlayerNearby = false;

    private void Awake()
    {
        if (_targetSprite == null) _targetSprite = GetComponent<SpriteRenderer>();

        if (_targetSprite != null)
        {
            _originalColor = _targetSprite.color;
        }
    }
    private void OnEnable()
    {
        _inputReader.InteractEvent += OnInteractInput;
        _inputReader.CancelEvent += CloseUI;
    }

    private void OnDisable()
    {
        _inputReader.InteractEndEvent -= OnInteractInput;
        _inputReader.InteractEndEvent -= CloseUI;

    }



    public void Interact()
    {
        _bannerController.OpenWithData(_bannerData);
    }

    public void InteractEnd()
    {

    }

    private void OnInteractInput()
    {
        if (_isPlayerNearby)
        {
            Interact();
        }
    }

    private void CloseUI()
    {
        _bannerController.CloseBanner();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerNearby = true;
            SetHighLight(true);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerNearby = false;
            SetHighLight(false);

        }
    }

    private void SetHighLight(bool isActive)
    {
        if (_targetSprite == null) return;

        if (isActive)
        {
            _targetSprite.color = _highlightColor;
        }
        else
        {
            _targetSprite.color = _originalColor;
        }
    }
}
