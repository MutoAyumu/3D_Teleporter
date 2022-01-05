using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressTheButtonScript : MonoBehaviour
{
    [SerializeField] Text _popUpText = default;
    [SerializeField] Transform _setPos = default;
    [SerializeField] string _settingButtonName = "Jump";
    [SerializeField] float _rayDistance = 1.5f;
    [SerializeField] LayerMask _hitLayer = default;

    RaycastHit _hit;

    void Update()
    {
        Press();
    }
    void Press()
    {
        var ray = Physics.Linecast(this.transform.position, _setPos.position, out _hit, _hitLayer);
        Debug.DrawLine(this.transform.position, _setPos.position, Color.blue);

        if(_popUpText && ray)
        {
            _popUpText.gameObject.SetActive(true);
        }
        else if(_popUpText && !ray)
        {
            _popUpText.gameObject.SetActive(false);
        }
        else
        {

        }

        if(Input.GetButtonDown(_settingButtonName) && ray)
        {
            var button = _hit.collider.GetComponent<PressedButtonScript>();

            if(button)
            {
                button.Pressed();
            }
        }
    }
}
