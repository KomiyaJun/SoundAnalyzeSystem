using UnityEngine;
using UnityEngine.Events;

public class ZoneTrigger : MonoBehaviour
{
    [Header("TagFilter")]
    [SerializeField] private string targetTag = "Player";

    [Header("Events")]
    public UnityEvent OnEnterZone;
    public UnityEvent OnExitZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            OnEnterZone.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            OnExitZone.Invoke();
        }
    }
}
