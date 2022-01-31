using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRobotScript : MonoBehaviour
{
    [SerializeField] Transform _rayPos = default;
    [SerializeField] Transform _setPos = default;
    [SerializeField] float _rayDirection = 100f;
    [SerializeField] LineRenderer _line = default;
    [SerializeField] float _lineWidth = 0.05f;
    [SerializeField] int _power = 1;
    [SerializeField] float _coolTime = 0.5f;
    Vector3 _lastPos = default;
    float _timer = default;
    bool isFire;

    private void Start()
    {
        _lastPos = _rayPos.position;
        _line.startWidth = _lineWidth;
        _line.endWidth = _lineWidth;
    }
    private void Update()
    {
        LaserPoint();
        CoolTime();
    }
    void LaserPoint()
    {
        RaycastHit hit;
        var ray = Physics.Raycast(_rayPos.position, _setPos.position, out hit, _rayDirection);

        if (!ray)//rayが何にも当たっていない時は
        {
            _line.SetPosition(1, _setPos.position);
        }
        else
        {
            if (!isFire)
            {
                Fire(hit);
            }
            _line.SetPosition(1, hit.point);
        }

        if(_lastPos != this.transform.position)//レーザーの原点の変更
        {
            _lastPos = this.transform.position;
            _line.SetPosition(0, _rayPos.position);
            Debug.Log("move");
        }
    }
    void Fire(RaycastHit hit)
    {
        var player = hit.collider.GetComponent<PlayerController>();

        if (player)//プレイヤーだったら
        {
            player.Damage(_power);
            isFire = true;
        }
    }
    void CoolTime()
    {
        if(isFire)
        {
            _timer += Time.deltaTime;

            if(_timer >= _coolTime)
            {
                isFire = false;
                _timer = 0;
            }
        }
    }
}
