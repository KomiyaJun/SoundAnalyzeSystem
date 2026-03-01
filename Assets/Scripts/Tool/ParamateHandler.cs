using UnityEngine;
using UnityEngine.Events;

public class ParamateHandler : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private InputReader _inputReader;

    [Header("つまみの設定")]
    [SerializeField, Range(0, 1)] private float currentValue = 0.3f;
    [SerializeField] private float changeValue = 0.1f;

    [Header("基準設定")]
    [SerializeField, Range(0.1f, 0.9f)] private float threshold = 0.5f;
    [SerializeField] private float baseOutPut = 1.0f;
    

    [Header("回転設定")]
    [SerializeField] private float minAngle = -45f;
    [SerializeField] private float maxAngle = 90f;

    [Header("出力値設定")]
    [SerializeField] private float minOutPut = 0.5f;
    [SerializeField] private float maxOutPut = 2.0f;

    [Header("イベント")]
    public UnityEvent<float> OnParamateChanged;

    private void Start()
    {
        UpdateVisualAndEvent();
    }

    private void OnEnable()
    {
        _inputReader.ParamateEvent += OnParamaterAdjust;
    }

    private void OnDisable()
    {
        _inputReader.ParamateEvent -= OnParamaterAdjust;
    }

    private void OnParamaterAdjust(float value)
    {
        currentValue = Mathf.Clamp(currentValue + value * changeValue, 0f, 1f);
        UpdateVisualAndEvent();
    }

    private void UpdateVisualAndEvent()
    {
        float targetAngle;
        float outputValue;

        if(currentValue <= threshold)
        {
            targetAngle = currentValue.Remap(0f, threshold, minAngle, 0f);
            outputValue = currentValue.Remap(0f, threshold, minOutPut, baseOutPut);
        }
        else
        {
            targetAngle = currentValue.Remap(threshold, 1f, 0f, maxAngle);
            outputValue = currentValue.Remap(threshold,1f,baseOutPut, maxOutPut);
        }
        //右回転
        this.transform.localEulerAngles = new Vector3(0, 0, -targetAngle);
        OnParamateChanged?.Invoke(outputValue);
    }
}
