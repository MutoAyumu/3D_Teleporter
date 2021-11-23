using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 5;
    [SerializeField] float _jumpPower = 5f;
    [SerializeField] float _groundRayLength = 1f; //接地判定用のRayの長さ

    Vector3 _dir;
    Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        InputMove();
        UpdateMove();
    }

    void InputMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        _dir = Vector3.forward * v + Vector3.right * h;
    }
    void UpdateMove()
    {
        if (_dir == Vector3.zero)
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        }
        else
        {
            _dir = Camera.main.transform.TransformDirection(_dir); //カメラを基準に座標をとる
            _dir.y = 0;

            Quaternion rotation = Quaternion.LookRotation(_dir);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime);

            Vector3 move = _dir.normalized * _moveSpeed;　//移動
            move.y = _rb.velocity.y;
            _rb.velocity = move;
        }
    }
}