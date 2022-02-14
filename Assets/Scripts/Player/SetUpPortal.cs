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
    [SerializeField] Animator _gunModel = default;
    [SerializeField] AudioSource _shootAudio = default;
    float _animSpeed;
    
    [SerializeField]PlayerController _player = default;
    GameManager _gmanager;
    bool isPause;
    private void Awake()
    {
        _gmanager = GameObject.FindObjectOfType<GameManager>();
        _animSpeed = _gunModel.speed;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position, this.transform.forward * _rayDistance);
    }
    private void Update()
    {
        if(isPause)
        {
            return;
        }

        if(Input.GetButtonDown("Fire1"))
        {
            FirePortal(0, this.transform.position, this.transform.forward, _rayDistance);
            _gunModel.SetTrigger("Shoot");
            _shootAudio.Play();
        }
        else if(Input.GetButtonDown("Fire2"))
        {
            FirePortal(1, this.transform.position, this.transform.forward, _rayDistance);
            _gunModel.SetTrigger("Shoot");
            _shootAudio.Play();
        }
    }
    void FirePortal(int portalID, Vector3 pos, Vector3 dir, float distance)
    {
        RaycastHit hit;
        Physics.Raycast(pos, dir, out hit, distance);

        if(hit.collider != null && hit.collider.gameObject.layer != _wallLayer)
        {
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
        isPause = true;
        _gunModel.speed = 0;
    }
    protected virtual void Resume()
    {
        isPause = false;
        _gunModel.speed = _animSpeed;
    }
}
