using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Scriptable Objects/MovementData")]
public class MovementData : ScriptableObject
{
    public float moveSpeed = 5;
    public float jumpForce = 10f;
    public float gravityScale = 1.0f;
    public float friction = 0.1f;
    public float acceleration = 1f;
}
