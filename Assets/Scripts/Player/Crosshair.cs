using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] PortalScript[] _portals = default;
    [SerializeField] Image[] _portalImage = default;

    private void Start()
    {
        _portalImage[0].color = _portals[0].PortalColor;
        _portalImage[1].color = _portals[1].PortalColor;

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
