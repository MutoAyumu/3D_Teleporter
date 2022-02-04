using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRobotScript : MonoBehaviour
{
    [SerializeField] Transform _rayPos = default;
    [SerializeField] float _rayDistance = 100f;
    [SerializeField] LineRenderer _line = default;
    [SerializeField] float _lineWidth = 0.05f;
    [SerializeField] int _power = 1;
    [SerializeField] float _coolTime = 0.5f;
    [SerializeField] float _groundRayDistance = 1;
    [SerializeField] LayerMask _groundLayer = default;
    [SerializeField] Animator _effect = default;
    [SerializeField] PlayerController _player = default;
    Vector3 _lastPos = default;
    float _timer = default;
    bool isFire;
    bool isGround = true;

    private void Start()
    {
        _lastPos = _rayPos.position;
        _line.SetPosition(0, _rayPos.position);
        _line.startWidth = _lineWidth;
        _line.endWidth = _lineWidth;
    }
    private void Update()
    {
        if (isGround)//一度でも接地判定がなくなれば動かないようにする
        {
            LaserPoint();
            IsGround();
            CoolTime();
        }
    }
    void LaserPoint()
    {
        RaycastHit hit;
        var ray = Physics.Raycast(_rayPos.position, _rayPos.forward, out hit, _rayDistance);
        Debug.DrawRay(_rayPos.position, _rayPos.forward * _rayDistance, Color.red);

        if (!ray)//rayが何にも当たっていない時は
        {
            _line.SetPosition(1, _rayPos.position + _rayPos.forward * _rayDistance);
            _effect.gameObject.SetActive(false);
        }
        else//rayが当たっているとき
        {
            if (!isFire)
            {
                Fire(hit);
            }
            _line.SetPosition(1, hit.point);
        }

        if(_lastPos != _rayPos.position)//レーザーの原点の変更
        {
            _lastPos = _rayPos.position;
            _line.SetPosition(0, _rayPos.position);
        }
    }
    void Fire(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Player"))//プレイヤーだったら
        {
            _player.Damage(_power);
            isFire = true;
            _effect.gameObject.SetActive(true);
        }
        else
        {
            _effect.gameObject.SetActive(false);
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
    void IsGround()
    {
        var ray = Physics.Raycast(this.transform.position, this.transform.up * -1, _groundRayDistance, _groundLayer);
        Debug.DrawRay(this.transform.position, this.transform.up * -1 * _groundRayDistance, Color.green);

        if (!ray)
        {
            isGround = false;
            _line.enabled = false;
            _effect.gameObject.SetActive(false);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && isGround)
        {
            var pos = _player.transform.position;
            pos.y = this.transform.position.y;
            this.transform.LookAt(pos, this.transform.up);//自身の向きを変更
        }
    }
}
