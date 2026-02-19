using UnityEngine;
using UnityEngine.InputSystem;
using System;

[CreateAssetMenu(menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, GameInput.IPlayerActions,GameInput.IUIActions,GameInput.IDialogueActions
{
    private GameInput _gameInput;

    private void OnEnable()
    {
        if(_gameInput == null)
        {
            _gameInput = new GameInput();

            _gameInput.Player.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);
            _gameInput.Dialogue.SetCallbacks(this);
        }
    }


    #region playerAction
    public event Action<Vector2> MoveEvent;
    public event Action DashStartEvent;
    public event Action DashEndEvent;

    public event Action JumpEvent;
    public event Action JumpEndEvent;

    public event Action AttackEvent;

    public event Action InteractEvent;
    public event Action InteractEndEvent;

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        MoveEvent?.Invoke(value);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            DashStartEvent?.Invoke();
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            DashEndEvent?.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            JumpEvent?.Invoke();
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            JumpEndEvent?.Invoke();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            InteractEvent?.Invoke();
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            InteractEndEvent?.Invoke();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        AttackEvent?.Invoke();
    }
    #endregion

    #region UIAction
    public event Action<Vector2> NavigateEvent;
    public event Action SubmitEvent;
    public event Action CancelEvent;

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        NavigateEvent?.Invoke(value);
    }
    public void OnSubmit(InputAction.CallbackContext context)
    {
        SubmitEvent?.Invoke();
    }
    public void OnCancel(InputAction.CallbackContext context)
    {
        CancelEvent?.Invoke();
    }
    #endregion

    #region DialogueAction
    public event Action NextEvent;

    public void OnNext(InputAction.CallbackContext context)
    {
        NextEvent?.Invoke();
    }
    #endregion
   
    //入力をプレイヤーに変更
    public void EnablePlayerEvent()
    {
        _gameInput.Player.Enable();
        _gameInput.UI.Disable();
        _gameInput.Dialogue.Disable();
    }
    //入力をUIに変更
    public void EnableUIEvent()
    {
        _gameInput.Player.Disable();
        _gameInput.UI.Enable();
        _gameInput.Dialogue.Disable();
    }
    //入力をダイアログに変更
    public void EnableDialogueEvent()
    {
        _gameInput.Player.Disable();
        _gameInput.UI.Disable();
        _gameInput.Dialogue.Enable();
    }
    
    //入力を全て無効化
    public void DisableAllInput() => _gameInput.Disable();

}
