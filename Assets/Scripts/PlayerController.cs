using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>プレイヤーにつける</summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 5;
    [SerializeField] float _jumpPower = 5f;
    [SerializeField] Vector3 _rayDistance = Vector3.zero;//接地判定用のRayの方向
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _eye;
    [SerializeField] string _portalTag = "";
    [SerializeField] AxisState _aimH;
    [SerializeField] AxisState _aimV;

    Vector3 _dir;
    Rigidbody _rb;
    bool isGround;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        InputMove();
        UpdateMove();
        IsGround();
    }
    /// <summary>動きの入力</summary>
    void InputMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        _dir = Vector3.forward * v + Vector3.right * h;
    }
    /// <summary>動きの更新</summary>
    void UpdateMove()
    {
        _aimH.Update(Time.deltaTime);
        _aimV.Update(Time.deltaTime);
        var aimRotationH = Quaternion.AngleAxis(_aimH.Value, Vector3.up);
        var aimRotationV = Quaternion.AngleAxis(_aimV.Value, Vector3.right);

        _eye.localRotation = aimRotationV;
        this.transform.rotation = aimRotationH;

        if (_dir == Vector3.zero)//止まっているとき
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        }
        else
        {
            if (!isGround)
            {
                return;
            }

            _dir = Camera.main.transform.TransformDirection(_dir); //カメラを基準に座標をとる
            _dir.y = 0;

            Vector3 move = _dir.normalized * _moveSpeed;　//移動
            move.y = _rb.velocity.y;
            _rb.velocity = move;
        }
    }
    /// <summary>接地判定</summary>
    void IsGround()
    {
        Debug.DrawLine(this.transform.position, this.transform.position + _rayDistance, Color.red);

        if (Physics.Linecast(this.transform.position, this.transform.position + _rayDistance, _groundLayer))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }
    public void ChangeRotation(float dir)
    {
        _aimH.Value = dir;
    }
}