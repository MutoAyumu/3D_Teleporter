using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressTheButtonScript : MonoBehaviour
{
    [SerializeField] Text _popUpText = default;
    [SerializeField] Transform _setPos = default;
    [SerializeField] string _settingButtonName = "E";
    [SerializeField] LayerMask _hitLayer = default;

    RaycastHit _hit;
    bool isHit;

    void Update()
    {
        Press();
    }
    void Press()
    {
        var ray = Physics.Linecast(this.transform.position, _setPos.position, out _hit, _hitLayer);
        Debug.DrawLine(this.transform.position, _setPos.position, Color.blue);

        if(ray)
        {
            if(Input.GetButtonDown(_settingButtonName) && _hit.collider.GetComponent<PressedButtonScript>())
            {
                var button = _hit.collider.GetComponent<PressedButtonScript>();
                button.Pressed();
            }
            if (_popUpText && !isHit)
            {
                _popUpText.gameObject.SetActive(true);
                isHit = true;
            }
        }
        else
        {
            if (_popUpText && isHit)
            {
                _popUpText.gameObject.SetActive(false);
                isHit = false;
            }
        }
    }
}
