using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 10f;

    [SerializeField] private LayerMask groundLayerMask;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void Initialize(Vector3 direction)
    {
        _rb.linearVelocity = direction.normalized * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isGroundLayer = (groundLayerMask.value & (1 << collision.gameObject.layer)) > 0;

        if(isGroundLayer || collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
