using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTest : MonoBehaviour
{
    private void Start()
    {
        var mtf = this.transform.localScale;
        transform.localScale = new Vector3(-mtf.x, mtf.y, mtf.z);
    }
    private void LateUpdate()
    {
        this.transform.LookAt(Camera.main.transform);
    }
}
