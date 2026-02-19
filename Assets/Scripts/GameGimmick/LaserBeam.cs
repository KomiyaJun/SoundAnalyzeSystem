using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private float maxDistance = 20f;   //ビームの最大の長さ
    [SerializeField] private LayerMask hitlayers;   //障害物となるレイヤー

    // Update is called once per frame
    void Update()
    {
        Vector2 startPos = transform.position;
        Vector2 direction = transform.up;

        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance, hitlayers);

        float beamLength;

        if(hit.collider != null)
        {
            beamLength = hit.distance;
        }
        else
        {
            beamLength = maxDistance;
        }

        transform.localScale = new Vector2(transform.localScale.x, beamLength);
    }
}
