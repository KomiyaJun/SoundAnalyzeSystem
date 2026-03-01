using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractUIHandler : MonoBehaviour, IInteractable
{
    [Header("éQè∆")]
    [SerializeField] private InputReader _inputReader;

    [Header("äJÇ≠UI")]
    [SerializeField] private Transform _ui;

    [Header("åıÇÈââèo")]
    [SerializeField] private SpriteRenderer _targetSprite;
    [SerializeField] private Color _highlightColor = new Color(1f, 1f, 0.7f);

    private Color _originalColor;
    private bool _isPlayerNearby = false;

    private void Awake()
    {
        if (_targetSprite == null) _targetSprite = GetComponent<SpriteRenderer>();

        if(_ui != null) _originalColor = _targetSprite.color;

        if(_ui != null) _ui.gameObject.SetActive(false);
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
        if(_ui != null)
        {
            bool isActive = _ui.gameObject.activeSelf;
            _ui.gameObject.SetActive(!isActive);

            //ì¸óÕÇÃêÿÇËë÷Ç¶
            if (!isActive) _inputReader.EnableUIEvent();
            if (isActive) _inputReader.EnablePlayerEvent();
        }
    }

    public void InteractEnd()
    {

    }

    private void OnInteractInput()
    {
        if( _isPlayerNearby)
        {
            Interact();
        }
    }

    private void CloseUI()
    {
        _ui.gameObject.SetActive(false);
        _inputReader.EnablePlayerEvent();
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

        if(isActive)
        {
            _targetSprite.color = _highlightColor;
        }
        else
        {
            _targetSprite.color = _originalColor;
        }
    }


}
