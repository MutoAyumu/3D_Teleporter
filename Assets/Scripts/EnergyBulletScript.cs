using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBulletScript : MonoBehaviour
{
    [SerializeField] float _bulletSpeed = 1f;
    [SerializeField] LayerMask _wallLayer = default;
    Rigidbody _rb;
    int _count;
    private void Start()
    {
        _rb = this.gameObject.GetComponent<Rigidbody>();
        _rb.velocity = this.transform.up * _bulletSpeed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();

        if(player)
        {
            //ここでプレイヤーに即死ダメージを与える
        }

        if(collision.gameObject.layer != _wallLayer)
        {
            _count++;

            if (_count >= 2)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            
        }
    }
}
