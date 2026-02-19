using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Rigidbody2D _rb;

    [SerializeField] private Transform _feetPos;

    [Header("Settings")]
    [SerializeField] private MovementData _groundSettings;
    [SerializeField] private MovementData _waterSettings;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckRadius = 0.2f;

    private IMovementLogic _currentLogic;
    private MovementData _currentData;
    private Vector2 _inputDirection;

    private bool _isGround;
    private bool _isRunning;
    private bool _canMove = true;
    private void Awake()
    {
        _currentLogic = new GroundMovement();
        _currentData = _groundSettings;
    }

    private void OnEnable()
    {
        _inputReader.MoveEvent += OnMoveInput;
        _inputReader.JumpEvent += OnJumpInput;
        _inputReader.JumpEndEvent += OnJumpEndInput;
        _inputReader.DashStartEvent += OnDashStart;
        _inputReader.DashEndEvent += OnDashEnd;
        _inputReader.EnablePlayerEvent();
        SetMovementState(new GroundMovement(), _groundSettings);
    }

    private void OnDisable()
    {
        _inputReader.MoveEvent -= OnMoveInput;
        _inputReader.JumpEvent -= OnJumpInput;
        _inputReader.JumpEndEvent -= OnJumpEndInput;
        _inputReader.DashStartEvent -= OnDashStart;
        _inputReader.DashEndEvent -= OnDashEnd;

    }

    private void OnMoveInput(Vector2 direction)
    {
        _inputDirection = direction;
    }

    private void OnDashStart()
    {
        _isRunning = true;
    }
    private void OnDashEnd()
    {
        _isRunning = false;
    }

    private void OnJumpInput()
    {
        if(_currentLogic != null)
        {
            _currentLogic.Jump(_rb, _currentData, _isGround);
        }
    }
    private void OnJumpEndInput()
    {
        if(_currentLogic != null)
        {
            _currentLogic.JumpEnd(_rb);
        }
    }

    private void Update()
    {

        _isGround = Physics2D.OverlapCircle(_feetPos.position, _groundCheckRadius, _groundLayer);
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;

        _currentLogic?.Move(_inputDirection, _rb, _currentData, _isRunning);
    }

    //èÛë‘êÿÇËë÷Ç¶
    private void SetMovementState(IMovementLogic newLogic, MovementData newData)
    {
        _currentLogic = newLogic;
        _currentData = newData;

        _currentLogic.Enter(_rb, _currentData);
    }


    public void EnterWaterState()
    {
        SetMovementState(new WaterMovement(), _waterSettings);
    }

    public void ExitWaterState()
    {
        SetMovementState(new GroundMovement(), _groundSettings);
    }

    public void SetControl(bool allow)
    {
        _canMove = allow;

        if (!_canMove)
        {
            _inputDirection = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
            _isRunning = false;
        }
    }
}
