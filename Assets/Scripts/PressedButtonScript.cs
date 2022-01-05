using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressedButtonScript : MonoBehaviour
{
    [SerializeField] UnityEvent _startEvent = default;
    [SerializeField] UnityEvent _endEvent = default;
    [SerializeField] float _intervalTime = 7f;

    bool isOn;
    float _timer;

    void Update()
    {
        if(isOn)
        {
            _timer += Time.deltaTime;

            if(_timer >= _intervalTime)
            {
                _endEvent.Invoke();
                isOn = false;
                _timer = 0;
            }
        }
    }
    public void Pressed()
    {
        _timer = 0;

        if (!isOn)
        {
            isOn = true;
            _startEvent.Invoke();
        }
    }
}
