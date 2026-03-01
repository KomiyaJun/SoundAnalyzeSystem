using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RespawnManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private PlayerMovementController _playerMover;

    [Header("初期チェックポイント")]
    [SerializeField] public Transform currentCheckPoint;

    public UnityEvent RespawnStartEvent;
    public UnityEvent RespawnEndEvent;

    public void StartRespawn()
    {
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        _playerMover.SetControl(false);  //移動入力を無効化
        RespawnStartEvent?.Invoke(); //演出等呼び出し

        yield return new WaitForSeconds(0.5f);

        transform.position = currentCheckPoint.position;

        yield return new WaitForSeconds(0.5f);

        RespawnEndEvent?.Invoke();  //演出等呼び出し

        _playerMover.SetControl(true);

    }
}
