using UnityEngine;

public class PhysicsMover : MonoBehaviour
{
    [Header("à íuê›íË")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("ìÆÇ´ê›íË")]
    [SerializeField] private float returnForce = 10f;
    [SerializeField] private float moveForce = 1.0f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        MoveTowardsA();
    }

    private void MoveTowardsA()
    {
        if (pointA == null) return;
        float distance = Vector2.Distance(transform.position, pointA.position);
        Vector2 directionToA = (pointA.position - transform.position).normalized;

        rb.AddForce(directionToA * returnForce * distance, ForceMode2D.Force);
    }

    public void TriggerMoveToB()
    {
        if (pointB == null) return;

        Vector2 directionToB = (pointB.position - transform.position).normalized;

        rb.AddForce(directionToB * moveForce, ForceMode2D.Impulse);
    }
}
