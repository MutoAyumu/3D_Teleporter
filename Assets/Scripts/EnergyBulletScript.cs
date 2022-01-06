using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBulletScript : MonoBehaviour
{
    [SerializeField] float _bulletSpeed = 1f;
    [SerializeField] LayerMask _portalLayer = default;
    Rigidbody _rb;
    private void Start()
    {
        _rb = this.gameObject.GetComponent<Rigidbody>();
        _rb.velocity = this.transform.up * _bulletSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();

        if(player)
        {
            //ここでプレイヤーに即死ダメージを与える
        }

        if(other.gameObject.layer != _portalLayer)
        {

        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
