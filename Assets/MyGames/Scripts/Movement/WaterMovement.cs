using UnityEngine;

public class WaterMovement : IMovementLogic
{
    public void Enter(Rigidbody2D rb, MovementData data)
    {
        rb.gravityScale = data.gravityScale;
        rb.linearDamping = data.drag;
    }

    public void Move(Vector2 direction, Rigidbody2D rb, MovementData data, bool isRunning)
    {
        rb.AddForce(direction * data.moveSpeed * 0.5f); //ˆÚ“®‚ğ’x‚­‚·‚é(…’†)
        rb.linearDamping = data.friction;   //…‚Ì’ïR
    }

    public void Jump(Rigidbody2D rb, MovementData data, bool isGround)
    {
        rb.AddForce(Vector2.up * data.jumpForce, ForceMode2D.Impulse);
    }

    public void JumpEnd(Rigidbody2D rb)
    {

    }

    public void Dash(Rigidbody2D rb)
    {

    }

}
