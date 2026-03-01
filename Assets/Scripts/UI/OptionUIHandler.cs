using UnityEngine;

public class OptionUIHandler : MonoBehaviour
{
    [Header("QÆ")]
    [SerializeField] InputReader _inputReader;

    [Header("ŠJ‚­UI")]
    [SerializeField] private Transform _ui;

    private void OnEnable()
    {
        _inputReader.OptionEvent += ChangeUI;
        _inputReader.CancelEvent += CloseUI;
    }

    private void OnDisable()
    {
        _inputReader.OptionEvent -= ChangeUI;
        _inputReader.CancelEvent -= CloseUI;
    }

    private void ChangeUI()
    {
        bool isActive = _ui.gameObject.activeSelf;
        _ui.gameObject.SetActive(!isActive);

        //“ü—Í‚ÌØ‚è‘Ö‚¦
        if (!isActive) _inputReader.EnableUIEvent();
        if (isActive) _inputReader.EnablePlayerEvent();

    }

    private void CloseUI()
    {
        _ui.gameObject.SetActive(false);
        _inputReader.EnablePlayerEvent();
    }
}
