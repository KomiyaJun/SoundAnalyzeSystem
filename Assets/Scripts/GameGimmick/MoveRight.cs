using System.Collections;
using UnityEngine;

public class MoveRight : MonoBehaviour
{
    [Header("ê›íË")]
    public float moveDistance = 5.0f;

    [Header("à⁄ìÆÇ…Ç©Ç©ÇÈéûä‘")]
    public float duration = 2.0f;

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
            elapsedTime += Time.deltaTime; //Timeégóp
            
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        transform.position = endPosition;
    }
}
