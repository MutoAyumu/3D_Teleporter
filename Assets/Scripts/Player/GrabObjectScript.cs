using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObjectScript : MonoBehaviour
{
    RaycastHit _hit;
    bool isGrab;
    Rigidbody _rb;
    Collider _col;
    GameObject _obj;
    float _halfScale;

    [SerializeField] LayerMask _objectLayer = default;
    [SerializeField] string _defaultLayerName = "Default";
    [SerializeField] string _gripLayerName = "IsGrip";
    [SerializeField] Transform _setPos = default;
    [SerializeField] string _settingButtonName = "E";

    private void Update()
    {
        GrabObject();
    }
    void GrabObject()
    {
        var ray = Physics.Linecast(this.transform.position, _setPos.position, out _hit, _objectLayer);
        Debug.DrawLine(this.transform.position, _setPos.position, Color.green);

        if(isGrab)//掴んでいるとき
        {
            if (!ray)
            {
                _obj.transform.position = _setPos.position;
            }
            else
            {
                _obj.transform.position = _hit.point + _hit.normal * _halfScale;
            }

            if(Input.GetButtonDown(_settingButtonName))
            {
                isGrab = false;
                _obj.layer = LayerMask.NameToLayer(_defaultLayerName);

                _rb.useGravity = true;
                _rb.isKinematic = false;

                _col.isTrigger = false;
            }
        }
        else//放しているとき
        {
            if(Input.GetButtonDown(_settingButtonName) && ray)
            {
                var collider = _hit.collider.gameObject.GetComponent<PortalableObject>();

                if (collider)
                {
                    isGrab = true;
                    _obj = _hit.collider.gameObject;
                    _obj.layer = LayerMask.NameToLayer(_gripLayerName);
                    _halfScale = _obj.transform.localScale.magnitude / 2;

                    _rb = _obj.GetComponent<Rigidbody>();
                    _rb.useGravity = false;
                    _rb.isKinematic = true;

                    _col = _obj.GetComponent<Collider>();
                    _col.isTrigger = true;
                }
            }
        }
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawRay(this.transform.position, this.transform.forward * _rayLength);
    //}
}
