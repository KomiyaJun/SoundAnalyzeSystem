using System.Collections;
using UnityEngine;

public class MoveRight : MonoBehaviour
{
    [Header("設定")]
    public float moveDistance = 5.0f;

    [Header("移動にかかる時間")]
    public float duration = 2.0f;

    [Header("プレイヤータグ")]
    [SerializeField] private string playerTag = "Player";

    // アプリ終了時に親子解除処理が走るのを防ぐためのフラグ
    private bool isQuitting = false;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public void StartMoving()
    {
        StartCoroutine(LerpMove());
    }

    IEnumerator LerpMove()
    {
        Vector3 startPosition = transform.position;

        Vector3 endPosition = startPosition + (Vector3.right * moveDistance);

        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; //Time使用
            
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        transform.position = endPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            collision.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // アプリ終了中、またはこのオブジェクト自体が非アクティブになる時は処理しない
        if (isQuitting || !gameObject.activeInHierarchy) return;

        if (collision.gameObject.CompareTag(playerTag))
        {
            collision.transform.SetParent(null);
        }
    }

}
