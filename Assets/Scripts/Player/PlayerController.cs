using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>プレイヤーにつける</summary>
public class PlayerController : PortalableObject
{
    [Space(10), Header("CameraMove")]
    [SerializeField]float _cameraSpeed = 3f;
    [SerializeField, Tooltip("XがMin　YがMax")] Vector2 _cameraValue = Vector2.zero;
    [SerializeField] float _slerpSpeed = 15f;
    [SerializeField] Transform _eye = default;
    [Space(10), Header("PlayerMove")]
    [SerializeField] float _moveSpeed = 3f;
    [Space(10), Header("IsGround")]
    [SerializeField] Transform _footPos = default;
    [SerializeField] Vector3 _rayDistance = Vector3.zero;
    [SerializeField] LayerMask _groundLayer = default;

    Quaternion _targetRotation;
    Vector3 _dir;
    Rigidbody _rb;
    bool isGround;

    public Quaternion TargetRotation { get => _targetRotation; set => _targetRotation = value; }
    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        TargetRotation = this.transform.rotation;
    }
    private void Update()
    {
        CameraMove();
        InputMove();
        UpdateMove();
        IsGround();
    }
    /// <summary>カメラの回転を制御する</summary>
    void CameraMove()
    {
        var rotation = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        var targetEuler = TargetRotation.eulerAngles + (Vector3)rotation * _cameraSpeed;

        if(targetEuler.x > 180.0f)
        {
            targetEuler.x -= 360.0f;
        }

        targetEuler.x = Mathf.Clamp(targetEuler.x, _cameraValue.x, _cameraValue.y);//指定した範囲の値に制限する為
        TargetRotation = Quaternion.Euler(targetEuler);//回転用のQuaternionを作成

        _eye.rotation = Quaternion.Slerp(_eye.rotation, TargetRotation, Time.deltaTime * _slerpSpeed);
    }
    void InputMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        _dir = Vector3.forward * v + Vector3.right * h;
    }
    /// <summary>動きの更新</summary>
    void UpdateMove()
    {
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

            _dir = _eye.transform.TransformDirection(_dir); //カメラを基準に座標をとる
            _dir.y = 0;

            Vector3 move = _dir.normalized * _moveSpeed;　//移動
            move.y = _rb.velocity.y;
            _rb.velocity = move;
        }
    }
    /// <summary>接地判定</summary>
    void IsGround()
    {
        Debug.DrawLine(_footPos.position, _footPos.position + _rayDistance, Color.red);

        if (Physics.Linecast(_footPos.position, _footPos.position + _rayDistance, _groundLayer))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }
    public override void Warp()
    {
        base.Warp();

        TargetRotation = Quaternion.LookRotation(_eye.transform.forward, Vector3.up);
    }
}