using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Rigidbody2D _rb;

    [Header("Settings")]
    [SerializeField] private MovementData _groundSettings;
    [SerializeField] private MovementData _waterSettings;

    private IMovementLogic _currentLogic;
    private MovementData _currentData;
    private Vector2 _inputDirection;


    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMoveInput;
        _inputReader.EnablePlayerEvent();
        SetMovementState(new GroundMovement(), _groundSettings);
    }

    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMoveInput;
    }

    private void OnMoveInput(Vector2 direction)
    {
        _inputDirection = direction;
    }

    private void FixedUpdate()
    {
        _currentLogic?.Move(_inputDirection, _rb, _currentData);
    }

    //èÛë‘êÿÇËë÷Ç¶
    private void SetMovementState(IMovementLogic newLogic, MovementData newData)
    {
        _currentLogic = newLogic;
        _currentData = newData;
    }


    public void EnterWaterState()
    {
        SetMovementState(new WaterMovement(), _waterSettings);
    }

    public void ExitWaterState()
    {
        SetMovementState(new WaterMovement(), _groundSettings);
    }

}
