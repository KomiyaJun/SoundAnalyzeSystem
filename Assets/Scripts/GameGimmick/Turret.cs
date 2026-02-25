using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("ê›íË")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _muzzlePoint;

    public void Fire()
    {
        if (_bulletPrefab == null || _muzzlePoint == null) return;

        Bullet bullet = Instantiate(_bulletPrefab, _muzzlePoint.position, _muzzlePoint.rotation);

        Vector3 shootDirection = _muzzlePoint.up;

        bullet.Initialize(shootDirection);
    }

}
