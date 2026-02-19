using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private float _interactionRadius = 1.5f;
    [SerializeField] private LayerMask _interactiveLayer;

    private IInteractable _activeInteractable;

    private void OnEnable()
    {
        _inputReader.InteractEvent += OnInteract;
        _inputReader.InteractEndEvent += OnInteractEnd;
    }

    private void OnDisable()
    {
        _inputReader.InteractEvent -= OnInteract;
        _inputReader.InteractEndEvent -= OnInteractEnd;
    }

    //インタラクトを行う
    private void OnInteract()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _interactionRadius, _interactiveLayer);

        foreach(var hit in hitColliders)
        {
            if(hit.TryGetComponent<IInteractable>(out var interactable))
            {
                _activeInteractable = interactable;
                _activeInteractable.Interact();
                break;
            }
        }
    }
    //インタラクトを行う
    private void OnInteractEnd()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _interactionRadius, _interactiveLayer);

        foreach(var hit in hitColliders)
        {
            if(hit.TryGetComponent<IInteractable>(out var interactable))
            {
                _activeInteractable.InteractEnd();
                _activeInteractable = null;
                break;
            }
        }
    }

    private void Update()
    {
        if(_activeInteractable != null)
        {
            float distance = Vector2.Distance(transform.position, ((MonoBehaviour)_activeInteractable).transform.position);

            if(distance > 1.0f)
            {
                OnInteractEnd();
            }
        }
    }

    // 判定範囲をエディタで見えるようにする
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);
    }
}
