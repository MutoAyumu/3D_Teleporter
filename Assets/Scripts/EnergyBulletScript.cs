using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBulletScript : MonoBehaviour
{
    [SerializeField] float _bulletSpeed = 1f;
    [SerializeField] LayerMask _portalLayer = default;
    [SerializeField] float _timeLimit = 0.1f;
    Rigidbody _rb;
    float _timer;
    bool _warp;
    private void Start()
    {
        _rb = this.gameObject.GetComponent<Rigidbody>();
        _rb.velocity = this.transform.up * _bulletSpeed;
    }
    private void Update()
    {
        if(_warp)
        {
            _timer += Time.deltaTime;

            if(_timer >= _timeLimit)
            {
                _warp = false;
                _timer = 0;
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!_warp)
        {
            var player = other.gameObject.GetComponent<PlayerController>();

            if (player)
            {
                //ここで殺す
                Debug.Log("kill");
                Destroy(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                Debug.Log("消えた");
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_warp)//ワープしていない時
        {
            if (other.gameObject.layer != _portalLayer)//wallLayerに当たった時
            {
                _warp = true;
                Debug.Log("hit");
            }
        }
    }
}
