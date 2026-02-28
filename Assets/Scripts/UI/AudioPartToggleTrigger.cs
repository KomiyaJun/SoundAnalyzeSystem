using MyGame.AudioSetting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class AudioPartToggleTrigger : MonoBehaviour
{
    [Header("òAågê›íË")]
    [SerializeField] private AudioAnalyzer _analyzer;
    [SerializeField] private BgmPartType _part;

    [Header("äOå©ê›íË")]
    [SerializeField] private Color _onColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color _offColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private Button _button;
    private Image _buttonImage;

    private void Start()
    {
        _button = GetComponent<Button>();
        _buttonImage = GetComponent<Image>();

        if (_analyzer == null) return;

        UpdateButtonColor();

        _button.onClick.AddListener(() =>
        {
            _analyzer.TogglePart(_part);
            UpdateButtonColor();
        });
    
    
    }

    private void UpdateButtonColor()
    {
        if(_analyzer.IsPartActive(_part))
        {
            _buttonImage.color = _onColor;
        }
        else
        {
            _buttonImage.color = _offColor;
        }
    }

    private void OnDestroy()
    {
        if(_button != null)
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}
