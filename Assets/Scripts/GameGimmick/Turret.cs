using UnityEngine;

public class Turret : MonoBehaviour
{
    private enum VelocityPoint
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    [Header("設定")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _muzzlePoint;

    [Header("発射方向")]
    [SerializeField] private VelocityPoint _velocity;

    private void OnValidate()
    {
        UpdateRotation();
    }

    public void Fire()
    {
        if (_bulletPrefab == null || _muzzlePoint == null) return;

        Bullet bullet = Instantiate(_bulletPrefab, _muzzlePoint.position, _muzzlePoint.rotation);



        Vector3 shootDirection = GetDirection();

        bullet.Initialize(shootDirection);
    }

    private void UpdateRotation()
    {
        // 方向ベクトルに合わせて、オブジェクトの角度(EulerAngles)を設定
        transform.eulerAngles = _velocity switch
        {
            VelocityPoint.UP => new Vector3(0, 0, 0),
            VelocityPoint.DOWN => new Vector3(0, 0, 180),
            VelocityPoint.LEFT => new Vector3(0, 0, 90),
            VelocityPoint.RIGHT => new Vector3(0, 0, -90),
            _ => Vector3.zero
        };
    }

    private Vector3 GetDirection()
    {
        return _velocity switch
        {
            VelocityPoint.UP => Vector3.up,
            VelocityPoint.DOWN => Vector3.down,
            VelocityPoint.RIGHT => Vector3.right,
            VelocityPoint.LEFT => Vector3.left,
            _ => Vector3.up
        };
    }

}
