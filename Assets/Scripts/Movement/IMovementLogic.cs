using UnityEngine;

public interface IMovementLogic
{
    void Move(Vector2 direction, Rigidbody2D rb, MovementData data);
}
