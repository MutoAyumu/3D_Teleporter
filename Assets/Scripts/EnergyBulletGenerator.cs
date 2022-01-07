using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBulletGenerator : MonoBehaviour
{
    [SerializeField] EnergyBulletScript _energyBullet = default;
    [SerializeField] EnergyBulletReceptor _bulletReceptor = default;
    [SerializeField] Transform _generatePoint = default;
    [SerializeField] float _limitTime = 6f;

    float _timer;

    private void Update()
    {
        if (!_bulletReceptor.IsHit)
        {
            _timer += Time.deltaTime;
        }

        if (_timer >= _limitTime)
        {
            Generate();
            _timer = 0;
        }
    }
    void Generate()
    {
        if(_energyBullet)
        {
            var obj = Instantiate(_energyBullet, _generatePoint.position, Quaternion.identity, this.transform);
            obj.transform.rotation = this.transform.rotation;
        }
    }
}
