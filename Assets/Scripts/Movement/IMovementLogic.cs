using UnityEngine;

public interface IMovementLogic
{
    void Enter(Rigidbody2D rb, MovementData data);

    void Move(Vector2 direction, Rigidbody2D rb, MovementData data, bool isRunning);

    void Jump(Rigidbody2D rb, MovementData data, bool isGround);

    void JumpEnd(Rigidbody2D rb);

    void Dash(Rigidbody2D rb);
}
