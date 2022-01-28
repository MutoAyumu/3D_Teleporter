using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnergyBulletReceptor : MonoBehaviour
{
    [SerializeField] LayerMask _bulletLayer = default;
    [SerializeField] UnityEvent _event = default;
    bool isHit;

    public bool IsHit { get => isHit; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != _bulletLayer)
        {
            isHit = true;
            Destroy(collision.gameObject);
            _event.Invoke();
        }
    }
}
