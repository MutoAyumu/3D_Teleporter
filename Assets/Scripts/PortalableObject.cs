using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalableObject : MonoBehaviour
{
    int _inPortalCount;
    PortalScript _inPortal = default;
    PortalScript _outPortal = default;
    Rigidbody _rb = default;
    protected Collider _collider = default;

    static readonly Quaternion _halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    public void SetInPortal(PortalScript inPortal, PortalScript outPortal, Collider wallCollider)
    {
        _inPortal = inPortal;
        _outPortal = outPortal;

        Physics.IgnoreCollision(_collider, wallCollider);

        ++_inPortalCount;
    }
    public void ExitPortal(Collider wallCollider)
    {
        Physics.IgnoreCollision(_collider, wallCollider, false);
        -- _inPortalCount;
    }
    public virtual void Warp()
    {
        var inTransform = _inPortal.transform;
        var outTransform = _outPortal.transform;

        Vector3 relativePos = inTransform.InverseTransformPoint(this.transform.position);
        relativePos = _halfTurn * relativePos;
        this.transform.position = outTransform.TransformPoint(relativePos);

        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * this.transform.rotation;
        relativeRot = _halfTurn * relativeRot;
        this.transform.rotation = outTransform.rotation * relativeRot;

        Vector3 relativeVel = inTransform.InverseTransformDirection(_rb.velocity);
        relativeVel = _halfTurn * relativeVel;
        _rb.velocity = outTransform.TransformDirection(relativeVel);

        var tmp = _inPortal;
        _inPortal = _outPortal;
        _outPortal = tmp;
    }
}
