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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != _bulletLayer)
        {
            isHit = true;
            _event.Invoke();
        }
    }
}
