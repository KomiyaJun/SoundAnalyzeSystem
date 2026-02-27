using UnityEngine;
using System.Collections.Generic;
public class VisualizerManager : MonoBehaviour
{
    [System.Serializable]
    public class VisualizerSettings
    {
        [Header("変形タイプ")]
        public VisualizerInstance.VisualizeType type = VisualizerInstance.VisualizeType.Scale;

        [Header("指定帯域(-1なら自動)")]
        public int minIndex = -1;
        public int maxIndex = -1;

        [Header("個別の反転設定")]
        public bool reverseValue = false;
    }

    [Header("生成設定")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private float spacing;
    [SerializeField] private bool reverse = false;
    [SerializeField] private List<VisualizerSettings> settingList = new List<VisualizerSettings>();

    [Header("共通パラメータ")]
    [SerializeField] private float baseMultiple = 1000f;
    [SerializeField] private float highFreqBoost = 10f;
    [SerializeField] private float lerpSpeed = 15f;
    [SerializeField] private float minHeight = 0.2f;
    [SerializeField] private float maxValue = 10f;

    void Start()
    {
        GenerateVisualizers();
    }

    private void GenerateVisualizers()
    {
        int count = settingList.Count;
        int maxPossibleFreq = 512;
        int step = maxPossibleFreq / count;

        for(int i = 0; i < count; i++)
        {
            var settings = settingList[i];

            int finalMin = (settings.minIndex == -1) ? (i * step) : settings.minIndex;
            int finalMax = (settings.minIndex == -1) ? ((i + 1) * step) : settings.maxIndex;

            int positionIndex = reverse ? (count - i) : i;
            Vector3 localPos = new Vector3(positionIndex * spacing, 0, 0);

            GameObject obj = Instantiate(prefab, transform);
            obj.transform.localPosition = localPos;
            obj.name = $"Visualizer_{i} (Range:{finalMin}-{finalMax})";

            var instance = obj.GetComponent<VisualizerInstance>();
            if(instance != null)
            {
                instance.SetUp(settings.type,finalMin, finalMax,baseMultiple, highFreqBoost, lerpSpeed, minHeight, maxValue,settings.reverseValue);
            }
        }
    }

    // ギズモ表示も反転に対応
    private void OnDrawGizmosSelected()
    {
        if (settingList == null || settingList.Count == 0) return;

        for (int i = 0; i < settingList.Count; i++)
        {
            // reverseフラグを見てギズモの描画位置も変える
            int positionIndex = reverse ? (settingList.Count - 1 - i) : i;
            Vector3 worldPos = transform.TransformPoint(new Vector3(positionIndex * spacing, 0, 0));

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(worldPos, Vector3.one * 0.5f);

#if UNITY_EDITOR
            // 設定リストの何番目がどこに配置されるかラベルを表示
            UnityEditor.Handles.Label(worldPos + Vector3.up * 0.5f, $"N[{i}]");
#endif
        }
    }
}
