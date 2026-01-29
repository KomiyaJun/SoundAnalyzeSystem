using UnityEngine;

public class WaterMovement : IMovementLogic
{
    public void Move(Vector2 direction, Rigidbody2D rb, MovementData data)
    {
        rb.AddForce(direction * data.moveSpeed * 0.5f); //ˆÚ“®‚ğ’x‚­‚·‚é(…’†)
        rb.linearDamping = data.friction;   //…‚Ì’ïR
    }
}
