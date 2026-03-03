using UnityEngine;
using UnityEngine.Events;

public class BeatCounter : MonoBehaviour
{
    [Header("カウンターマックス時")]
    public UnityEvent CounterReached;

    [Header("初期化時")]
    public UnityEvent<int> OnInitialized;

    [Header("カウンター増加時")]
    public UnityEvent<int> OnCounterChanged;

    [Header("設定")]
    [SerializeField] private int targetCount = 3;

    [Space(30),Tooltip("確認用")]
    [SerializeField, ReadOnly]private int _currentCount = 0;

    public void Count()
    {
        _currentCount++;
        OnCounterChanged?.Invoke(_currentCount);

        if(_currentCount > targetCount)
        {
            CounterReached?.Invoke();
            _currentCount = 0;
            OnCounterChanged?.Invoke(_currentCount);
        }
    }

    private void Start()
    {
        OnInitialized?.Invoke(targetCount);
        OnCounterChanged?.Invoke(_currentCount);
    }

}
