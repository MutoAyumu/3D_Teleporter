using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    [SerializeField] LineRenderer _line = default;
    [SerializeField] float _rayDistance = 10;
    [SerializeField] LayerMask _layerMask = default;

    private void Update()
    {
        DrawLine();
    }
    void DrawLine()
    {
        RaycastHit hit;
        var point = Physics.Raycast(this.transform.position, this.transform.up, out hit, _rayDistance, _layerMask);
        Debug.DrawRay(this.transform.position, this.transform.up * _rayDistance, Color.red);

        if (point)
        {
            _line.SetPosition(0, this.transform.position);
            _line.SetPosition(1, hit.point);
        }
    }
}
