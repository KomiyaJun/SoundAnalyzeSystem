using UnityEngine;

public class GroundMovement : MonoBehaviour, IMovementLogic
{
    public void Enter(Rigidbody2D rb, MovementData data)
    {
        rb.gravityScale = data.gravityScale;
        rb.linearDamping = data.drag;
    }

    //TimeŽg—p
    public void Move(Vector2 direction, Rigidbody2D rb, MovementData data, bool isRunning)
    {
        float currentSpeed = isRunning ? data.runSpeed : data.moveSpeed;
        float targetX = direction.x * currentSpeed;

        Vector2 targetVelocity = new Vector2(targetX, rb.linearVelocityY);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity,data.acceleration * Time.fixedDeltaTime);
    }

    public void Jump(Rigidbody2D rb, MovementData data, bool isGround)
    {
        if (isGround)
        {
            rb.AddForce(Vector2.up * data.jumpForce, ForceMode2D.Impulse);
        }
    }

    public void JumpEnd(Rigidbody2D rb)
    {
        if(rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);
        }
    }

    public void Dash(Rigidbody2D rb)
    {

    }
}
