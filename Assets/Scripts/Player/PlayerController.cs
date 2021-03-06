using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>プレイヤーにつける</summary>
public class PlayerController : PortalableObject
{
    [Space(10), Header("CameraMove")]
    [SerializeField] float _cameraSpeed = 3f;
    [SerializeField, Tooltip("XがMin　YがMax")] Vector2 _cameraValue = Vector2.zero;
    [SerializeField] float _slerpSpeed = 15f;
    [SerializeField] Transform _eye = default;
    [SerializeField] GameObject _body = default;
    [SerializeField] Animator _gunModel = default;
    [SerializeField] Animator _playerAnim = default;
    [Space(10), Header("PlayerMove")]
    [SerializeField] float _moveSpeed = 3f;
    [SerializeField] float _jumpPower = 15f;
    [SerializeField] string _settingButtonName = "Jump";
    [SerializeField] float _hitPoint = 100;
    [SerializeField, Tooltip("ダメージを受けてから回復するまでの時間")] float _recoveryTime = 7f;
    [SerializeField, Tooltip("全回復するまでの時間")] float _recoverPoint = 5f;
    [Space(10), Header("IsGround")]
    [SerializeField] Transform _footPos = default;
    [SerializeField] Vector3 _rayDistance = Vector3.zero;
    [SerializeField] LayerMask _groundLayer = default;
    [SerializeField] PostProcessVolume _playerVolume = default;
    [SerializeField] string _respawnTag = "Respawn";
    [SerializeField] Image _respawnPanel = default;

    Vignette _vignette;
    ChromaticAberration _chromatic;

    Quaternion _targetRotation;
    Vector3 _dir;
    Vector3 _respawnPos;
    bool isGround;
    bool isPause;
    bool isDead;
    bool isDamage;
    bool isRecover;
    Animator _anim;
    float _animSpeed;
    float _currentHP;
    float _timer;

    public Quaternion TargetRotation { get => _targetRotation;}
    protected override void Awake()
    {
        base.Awake();
        _targetRotation = this.transform.rotation;
        _anim = _body.GetComponent<Animator>();
        _animSpeed = _anim.speed;
        _currentHP = _hitPoint;
        var profile = _playerVolume.profile;
        _vignette = profile.GetSetting<Vignette>();
        _chromatic = profile.GetSetting<ChromaticAberration>();
        _respawnPos = this.transform.position;
        _respawnPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isPause && !isDead)
        {
            CameraMove();
            InputMove();
            UpdateMove();
            InputJump();
            IsGround();
            Recovery();
        }
    }
    /// <summary>カメラの回転を制御する</summary>
    void CameraMove()
    {
        var rotation = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        var targetEuler = TargetRotation.eulerAngles + (Vector3)rotation * _cameraSpeed;

        if (targetEuler.x > 180.0f)
        {
            targetEuler.x -= 360.0f;
        }

        targetEuler.x = Mathf.Clamp(targetEuler.x, _cameraValue.x, _cameraValue.y);//指定した範囲の値に制限する為
        _targetRotation = Quaternion.Euler(targetEuler);//回転用のQuaternionを作成

        _eye.rotation = Quaternion.Slerp(_eye.rotation, TargetRotation, Time.deltaTime * _slerpSpeed);
        var dir = _eye.rotation;
        dir.x = _body.transform.rotation.x;
        dir.z = _body.transform.rotation.z;
        _body.transform.rotation = dir;
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
            _gunModel.SetBool("Walk", false);

            if (isGround)
            {
                _anim.SetBool("Walk", false);
            }
        }
        else
        {
            _dir = _eye.transform.TransformDirection(_dir); //カメラを基準に座標をとる
            _dir.y = 0;

            Vector3 move = _dir.normalized * _moveSpeed;　//移動
            move.y = _rb.velocity.y;
            _rb.velocity = move;

            if (isGround)
            {
                _anim.SetBool("Walk", true);
                _gunModel.SetBool("Walk", true);
            }
        }
    }
    void InputJump()
    {
        if (isGround)
        {
            if (Input.GetButtonDown(_settingButtonName))
            {
                _rb.AddForce(this.transform.up * _jumpPower, ForceMode.Impulse);
            }
        }
    }
    /// <summary>接地判定</summary>
    void IsGround()
    {
        Debug.DrawLine(_footPos.position, _footPos.position + _rayDistance, Color.red);

        if (Physics.Linecast(_footPos.position, _footPos.position + _rayDistance, _groundLayer))
        {
            isGround = true;
            _anim.SetBool("IsGround", true);
        }
        else
        {
            isGround = false;
            _anim.SetBool("IsGround", false);
        }
    }
    public void Damage(float damage)
    {
        if (_currentHP >= 0)//体力が0以上の時は計算する
        {
            _currentHP -= damage;
            isDamage = true;
            _timer = 0;

            if (_vignette.intensity.value <= 0.5f)
            {
                _vignette.intensity.value += 0.5f / _hitPoint;
            }
            if (_chromatic.intensity.value <= 1)
            {
                _chromatic.intensity.value += 1 / _hitPoint;
            }
            Debug.Log("HP" + _currentHP + "Vignette" + _vignette.intensity.value + "Chromatic" + _chromatic.intensity.value);

            if(_currentHP <= 0)
            {
                Debug.Log("Dead");
                isDead = true;
                DOVirtual.Float(_vignette.intensity.value, 0.5f, 0.8f, value => _vignette.intensity.value = value);
                DOVirtual.Float(_chromatic.intensity.value, 1f, 0.8f, value => _chromatic.intensity.value = value);
                _playerAnim.SetBool("Dead", isDead);
                _rb.velocity = Vector3.zero;
                _body.SetActive(false);
                _respawnPanel.gameObject.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    void Recovery()
    {
        if (isDamage)
        {
            _timer += Time.deltaTime;

            if (_timer >= _recoveryTime)//タイマーの方が大きくなったら回復を始める
            {
                _timer = 0;
                isDamage = false;
                isRecover = true;
                Debug.Log("回復を始めます");
            }
        }
        else if (!isDamage && isRecover)
        {
            _currentHP += _hitPoint / _recoverPoint * Time.deltaTime;

            if (_vignette.intensity.value > 0)
            {
                _vignette.intensity.value -= 0.5f / _recoverPoint * Time.deltaTime;
            }
            if (_chromatic.intensity.value > 0)
            {
                _chromatic.intensity.value -= 1 / _recoverPoint * Time.deltaTime;
            }

            Debug.Log("HP" + _currentHP + " Vignette" + _vignette.intensity.value + " Chromatic" + _chromatic.intensity.value);

            if (_currentHP > _hitPoint)
            {
                isRecover = false;
                _currentHP = _hitPoint;
                _chromatic.intensity.value = 0;
                _vignette.intensity.value = 0;
                Debug.Log("全回復しました");
            }
        }
    }
    public void ReStart()
    {
        isDead = false;
        _playerAnim.SetBool("Dead", isDead);
        _currentHP = _hitPoint;
        _vignette.intensity.value = 0;
        _chromatic.intensity.value = 0;
        _body.SetActive(true);
        this.transform.position = _respawnPos;
        Debug.Log("リスタートしました");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(_respawnTag))
        {
            _respawnPos = this.transform.position;
            Debug.Log("リスポーン位置を変更しました : " + _respawnPos);
        }
    }
    public override void Warp()
    {
        base.Warp();
        _targetRotation = Quaternion.LookRotation(_eye.transform.forward, Vector3.up);
    }
    protected override void Pause()
    {
        isPause = true;
        base.Pause();
        _rb.isKinematic = true;
        _anim.speed = 0;
    }
    protected override void Resume()
    {
        _rb.isKinematic = false;
        base.Resume();
        isPause = false;
        _anim.speed = _animSpeed;
    }
}