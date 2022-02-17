using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrabObjectScript : MonoBehaviour
{
    RaycastHit _hit;
    bool isGrab;
    bool isHit;
    Rigidbody _rb;
    Collider _col;
    GameObject _obj;

    [SerializeField] LayerMask _objectLayer = default;
    [SerializeField] LayerMask _wallLayer = default;
    [SerializeField] Transform _setPos = default;
    [SerializeField] string _settingButtonName = "E";
    [SerializeField] Text _popUpText = default;

    private void Update()
    {
        GrabObject();
    }
    void GrabObject()
    {
        var ray = Physics.Linecast(this.transform.position, _setPos.position, out _hit, _objectLayer);//オブジェクトを持つためのレイ
        Debug.DrawLine(this.transform.position, _setPos.position, Color.green);

        if(isGrab)//掴んでいるとき
        {
            RaycastHit hit;
            var wallHit = Physics.Linecast(this.transform.position, _setPos.position, out hit, _wallLayer);

            if (!wallHit)
            {
                _obj.transform.position = _setPos.position;//対象の座標をsetPosにする
            }
            else
            {
                _obj.transform.position = hit.point;
            }

            if(Input.GetButtonDown(_settingButtonName))
            {
                isGrab = false;

                _rb.useGravity = true;
                _rb.isKinematic = false;

                _col.isTrigger = false;
            }
        }
        else//放しているとき
        {
            if(ray)
            {
                if (Input.GetButtonDown(_settingButtonName) && _hit.collider.gameObject.GetComponent<PortalableObject>())
                {
                    var collider = _hit.collider.gameObject.GetComponent<PortalableObject>();

                    isGrab = true;
                    _obj = _hit.collider.gameObject;

                    _rb = _obj.GetComponent<Rigidbody>();
                    _rb.useGravity = false;
                    _rb.isKinematic = true;

                    _col = _obj.GetComponent<Collider>();
                    _col.isTrigger = true;
                }

                if (_popUpText && !isHit)
                {
                    _popUpText.gameObject.SetActive(true);
                    isHit = true;
                }
            }
            else
            {
                if (_popUpText && isHit)
                {
                    _popUpText.gameObject.SetActive(false);
                    isHit = false;
                }
            }
        }
    }
}
