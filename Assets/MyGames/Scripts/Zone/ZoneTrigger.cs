using UnityEngine;
using UnityEngine.Events;
using MyGame.AudioSetting;

public class ZoneTrigger : MonoBehaviour
{
    [Header("TagFilter")]
    [SerializeField] private string targetTag = "Player";

    [Header("Events")]
    public UnityEvent OnEnterZone;
    public UnityEvent OnExitZone;

    private bool isEntered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            OnEnterZone.Invoke();
            isEntered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            OnExitZone.Invoke();
            isEntered = false;
        }
    }
}
