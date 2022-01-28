using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBulletScript : MonoBehaviour
{
    [SerializeField] float _bulletSpeed = 1f;
    [SerializeField] LayerMask _portalLayer = default;
    Rigidbody _rb;
    bool _warp;
    private void Start()
    {
        _rb = this.gameObject.GetComponent<Rigidbody>();
        _rb.velocity = this.transform.up * _bulletSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if (_warp)//ワープしているとき
        {
            if (other.gameObject.layer != _portalLayer)
            {
                _warp = false;
                Debug.Log("当たっている");
            }
        }
        else if (!_warp)//ワープしていない時
        {
            if (other.gameObject.layer != _portalLayer)
            {
                _warp = true;
                Debug.Log("当たっていない");
            }
            else
            {
                var player = other.gameObject.GetComponent<PlayerController>();

                if (player)
                {
                    //ここで殺す
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
