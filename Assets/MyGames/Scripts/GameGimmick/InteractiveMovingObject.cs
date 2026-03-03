using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveMovingObject : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private InputReader _inputReader;

    [Header("移動の端点")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("パラメータ設定")]
    [SerializeField] private float paramA;//A地点での値
    [SerializeField] private float paramB;//B地点での値

    [Header("移動速度")]
    [SerializeField] private float moveSpeed = 1f;

    [Header("イベント")]
    public UnityEvent<float> OnValueChanged;
    
    private float _lastParam;

    [Range(0, 1)]
    public float currentRatio = 0f;
    public float CurrentParam { get; private set; }

    private Vector2 inputDirection;

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMoveInput;
    }
    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMoveInput;
    }

    private void Update()
    {
        transform.position = Vector2.Lerp(pointA.position, pointB.position, currentRatio);

        CurrentParam = currentRatio.Remap(0, 1f, paramA, paramB);

        if(!Mathf.Approximately(CurrentParam, _lastParam))
        {
            OnValueChanged?.Invoke(CurrentParam);
            _lastParam = CurrentParam;
        }
    }
    
    public void HandlePush()
    {
        Vector2 directionAB = (pointB.position - pointA.position).normalized;

        float Dot = Vector2.Dot(inputDirection, directionAB);

        if(Dot > 0.5f)
        {
            MoveTowardsB();
            Debug.Log("B");
        }
        else if(Dot < -0.5f)
        {
            MoveTowardsA();
            Debug.Log("A");
        }
    }

    public void MoveTowardsA()
    {
        currentRatio = Mathf.Clamp01(currentRatio - moveSpeed * Time.deltaTime);
    }

    public void MoveTowardsB()
    {
        currentRatio = Mathf.Clamp01(currentRatio + moveSpeed * Time.deltaTime);
    }

    

    private void OnMoveInput(Vector2 direction)
    {
        inputDirection = direction;
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.3f);
            Gizmos.DrawWireSphere(pointB.position, 0.3f);
        }
    }
}
