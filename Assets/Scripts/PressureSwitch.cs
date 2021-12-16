using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PressureSwitch : MonoBehaviour
{
    float _totalMass;
    Collider _collider;
    Animator _anim;
    [SerializeField] float _pressLimit = 5;

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
            }
        }
    }
}
