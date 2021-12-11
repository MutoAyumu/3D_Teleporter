using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpPortal : MonoBehaviour
{
    [SerializeField] PortalScript[] _portals = default;
    [SerializeField] LayerMask _wallLayer = default;
    [SerializeField] LayerMask _portalLayer = default;
    [SerializeField] Crosshair _crosshair = default;
    [SerializeField] float _rayDistance = 50f;
    
    [SerializeField]PlayerController _player = default;
    private void Awake()
    {
        //_player = GetComponent<PlayerController>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position, this.transform.forward * _rayDistance);
    }
    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            FirePortal(0, this.transform.position, this.transform.forward, _rayDistance);
        }
        else if(Input.GetButtonDown("Fire2"))
        {
            FirePortal(1, this.transform.position, this.transform.forward, _rayDistance);
        }
    }
    void FirePortal(int portalID, Vector3 pos, Vector3 dir, float distance)
    {
        RaycastHit hit;
        Physics.Raycast(pos, dir, out hit, distance, _wallLayer);

        if(hit.collider != null)
        {
            if(hit.collider.gameObject.layer == _portalLayer)
            {
                //ここ必要かわからんから後回しにするわ
            }

            //カメラの向きをポータルの方向と合わせる
            var cameraRotation = _player.TargetRotation;
            var portalRight = cameraRotation * Vector3.right;

            if (Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
            {
                portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
            }
            else
            {
                portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;
            }

            var portalForward = -hit.normal;
            var portalUp = -Vector3.Cross(portalRight, portalForward);

            var portalRotation = Quaternion.LookRotation(portalForward, portalUp);

            bool wasPlaced = _portals[portalID].PlacePortal(hit.collider, hit.point, portalRotation);

            if(wasPlaced)
            {
                _crosshair.SetPortal(portalID, true);//クロスヘアのImageを変更する
            }
        }
    }
}
