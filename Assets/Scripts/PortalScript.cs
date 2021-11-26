using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PortalScript : MonoBehaviour
{
    [SerializeField] LayerMask _wallLayer = default;
    [SerializeField] float _RayLength = 5f;
    [SerializeField] float _hitRayOffset = 0.01f;
    [SerializeField] Canvas[] _portalImage = default;
    int _count;
    bool isTeleport;
    RaycastHit hit;

    private void Update()
    {
        SetPortal();

        if (!Input.GetButtonDown("Cancel"))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    void SetPortal()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _RayLength, _wallLayer))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

            if(Input.GetButtonDown("Fire1"))
            {
                _count++;

                if (_count % 2 != 0)
                {
                    _portalImage[0].transform.rotation = Quaternion.LookRotation(hit.normal);
                    _portalImage[0].transform.position = hit.point + (hit.normal * _hitRayOffset);
                }
                else
                {
                    _portalImage[1].transform.rotation = Quaternion.LookRotation(hit.normal);
                    _portalImage[1].transform.position = hit.point + (hit.normal * _hitRayOffset);
                }
            }
        }
    }
    void ResetPortal()
    {
        
    }
    /// <summary>テレポート用の関数</summary>
    /// <param name="other"></param>
    /// <param name="Pos"></param>
    public void Teleport(Collider other, Transform Pos)
    {
        if (!isTeleport)//入った時
        {
            isTeleport = true;
            other.transform.position = Pos.transform.position;
        }
        else//出たとき
        {
            isTeleport = false;
        }
    }
}
