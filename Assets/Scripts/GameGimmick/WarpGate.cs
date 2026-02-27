using UnityEngine;

public class WarpGate : MonoBehaviour, IInteractable
{
    [Header("éQè∆")]
    [SerializeField] private InputReader _inputReader;

    [Header("ÉèÅ[Évê›íË")]
    [SerializeField] private Transform destination;

    private bool _isPlayerInSide = false;

    private void OnEnable()
    {
        _inputReader.InteractEvent += OnInteractTriggered;
    }

    private void OnDisable()
    {
        _inputReader.InteractEndEvent -= OnInteractTriggered;
    }



    public void Interact()
    {
        if (destination == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            Warp(player);
        }
    }

    private void Warp(GameObject player)
    {
        player.transform.position = destination.position;
    }

    public void InteractEnd()
    {

    }

    private void OnInteractTriggered()
    {
        if (_isPlayerInSide)
        {
            Interact();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInSide = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInSide= false;
        }
    }
}
