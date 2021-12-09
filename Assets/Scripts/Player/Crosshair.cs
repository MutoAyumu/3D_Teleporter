using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] Image[] _portalImage = default;

    private void Start()
    {
        _portalImage[0].gameObject.SetActive(false);
        _portalImage[1].gameObject.SetActive(false);
    }
    public void SetPortal(int portalID, bool isSet)
    {
        if(portalID == 0)
        {
            _portalImage[0].gameObject.SetActive(isSet);
        }
        else
        {
            _portalImage[1].gameObject.SetActive(isSet);
        }
    }
}
