using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class PressureSwitch : MonoBehaviour
{
    float _totalMass;
    Collider _collider;
    Animator _anim;
    [SerializeField] float _pressLimit = 5;
    [SerializeField] UnityEvent _pressedEvent = default;
    [SerializeField] UnityEvent _SeparatedEvent = default;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
        _anim = this.transform.parent.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<Rigidbody>();

        if (obj != null)
        {
            _totalMass += obj.mass;
            Debug.Log(_totalMass);

            if(_totalMass >= _pressLimit)
            {
                _anim.SetBool("Press", true);
                _pressedEvent.Invoke();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<Rigidbody>();

        if (obj != null)
        {            
            _totalMass -= obj.mass;
            Debug.Log(_totalMass);

            if(_totalMass <= 0)
            {
                _anim.SetBool("Press", false);
                _SeparatedEvent.Invoke();
            }
        }
    }
}
