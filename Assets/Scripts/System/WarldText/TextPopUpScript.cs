using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPopUpScript : MonoBehaviour
{
    [SerializeField] Animator _popUpPanel = default;
    [SerializeField] string _playerTag = "Player";
    private void Start()
    {
        _popUpPanel.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(_playerTag))
        {
            if(!_popUpPanel.gameObject.activeSelf)
            {
                _popUpPanel.gameObject.SetActive(true);
            }

            _popUpPanel.SetBool("Active",true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(_playerTag))
        {
            _popUpPanel.SetBool("Active", false);
        }
    }
}
