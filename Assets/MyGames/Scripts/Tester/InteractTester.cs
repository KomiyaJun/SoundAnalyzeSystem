using UnityEngine;
using UnityEngine.Events;

public class InteractTester : MonoBehaviour, IInteractable
{
    public UnityEvent InteractEvent;
    public UnityEvent InteractingEvent;
    public UnityEvent InteractEndEvent;

    private bool isInteracting = false;
    public void Interact()
    {
        if (!isInteracting)
        {
            Debug.Log("インタラクト！");
            InteractEvent?.Invoke();
            isInteracting = true;
        }
    }
    public void InteractEnd()
    {   
        Debug.Log("インタラクトエンド！");
        InteractEndEvent?.Invoke();
        isInteracting = false;
    }
    private void Update()
    {
        if (isInteracting)
        {
            InteractingEvent?.Invoke();
        }
    }
}
