using UnityEngine;

public class GroundMovement : MonoBehaviour, IMovementLogic
{
    //Timeégóp
    public void Move(Vector2 direction, Rigidbody2D rb, MovementData data)
    {
        Vector2 targetVelocity = new Vector2(direction.x * data.moveSpeed, rb.linearVelocityY);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity,data.acceleration * Time.fixedDeltaTime);
    }
}
