using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalableObject : MonoBehaviour
{
    int _inPortalCount;
    protected PortalScript _inPortal = default;
    protected PortalScript _outPortal = default;
    protected Rigidbody _rb = default;
    protected Collider _collider = default;
    protected GameManager _gmanager = default;
    Vector3 _angularVelocity;
    Vector3 _velocity;
    protected GameObject _cloneObject;

    protected static readonly Quaternion _halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _gmanager = GameObject.FindObjectOfType<GameManager>();

        _cloneObject = new GameObject();
        _cloneObject.SetActive(false);
        var meshFilter = _cloneObject.AddComponent<MeshFilter>();
        var meshRenderer = _cloneObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = GetComponent<MeshFilter>().mesh;
        meshRenderer.materials = GetComponent<MeshRenderer>().materials;
        _cloneObject.transform.localScale = transform.localScale;
        _cloneObject.name = this.gameObject.name + "clone";
    }
    protected virtual void LateUpdate()
    {
        if (_inPortal == null || _outPortal == null)
        {
            return;
        }

        if (_cloneObject.activeSelf && _inPortal.IsPlaced() && _outPortal.IsPlaced())
        {
            var inTransform = _inPortal.transform;
            var outTransform = _outPortal.transform;

            // クローンの位置を更新
            Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
            relativePos = _halfTurn * relativePos;
            _cloneObject.transform.position = outTransform.TransformPoint(relativePos);

            // クローンの回転を更新
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
            relativeRot = _halfTurn * relativeRot;
            _cloneObject.transform.rotation = outTransform.rotation * relativeRot;
        }
        else
        {
            _cloneObject.transform.position = new Vector3(-1000.0f, 1000.0f, -1000.0f);
        }
    }
    public void SetInPortal(PortalScript inPortal, PortalScript outPortal, Collider wallCollider)
    {
        _inPortal = inPortal;
        _outPortal = outPortal;

        Physics.IgnoreCollision(_collider, wallCollider);

        ++_inPortalCount;
        _cloneObject.SetActive(true);
    }
    public void ExitPortal(Collider wallCollider)
    {
        Physics.IgnoreCollision(_collider, wallCollider, false);
        --_inPortalCount;

        if (_inPortalCount == 0)
        {
            _cloneObject.SetActive(false);
        }
    }
    public virtual void Warp()
    {
        var inTransform = _inPortal.transform;
        var outTransform = _outPortal.transform;

        Vector3 relativePos = inTransform.InverseTransformPoint(this.transform.position);
        relativePos = _halfTurn * relativePos;
        this.transform.position = outTransform.TransformPoint(relativePos);

        var inRot = inTransform.rotation;
        inRot.x = 0;
        inRot.z = 0;
        var outRot = outTransform.rotation;
        outRot.x = 0;
        outRot.z = 0;
        Quaternion relativeRot = Quaternion.Inverse(inRot) * this.transform.rotation;
        relativeRot = _halfTurn * relativeRot;
        this.transform.rotation = outRot * relativeRot;

        var warpPower = 1.05f;
        Vector3 relativeVel = inTransform.InverseTransformDirection(_rb.velocity);
        relativeVel = _halfTurn * relativeVel;
        _rb.velocity = outTransform.TransformDirection(relativeVel) * warpPower;

        var tmp = _inPortal;
        _inPortal = _outPortal;
        _outPortal = tmp;
    }
    private void OnEnable()
    {
        if (_gmanager)
        {
            _gmanager.OnPauseResume += PauseResume;
        }
    }
    private void OnDisable()
    {
        if (_gmanager)
        {
            _gmanager.OnPauseResume -= PauseResume;
        }
    }
    void PauseResume(bool isPause)
    {
        if (isPause)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
    protected virtual void Pause()
    {
        // 速度・回転を保存し、Rigidbody を停止する
        _angularVelocity = _rb.angularVelocity;
        _velocity = _rb.velocity;
        _rb.Sleep();
    }
    protected virtual void Resume()
    {
        // Rigidbody の活動を再開し、保存しておいた速度・回転を戻す
        _rb.WakeUp();
        _rb.angularVelocity = _angularVelocity;
        _rb.velocity = _velocity;
    }
    private void OnDestroy()
    {
        Destroy(_cloneObject);
    }
}
