using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var h = Input.GetAxis("Mouse X");
        var v = Input.GetAxis("Mouse Y");

        if(Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(h);
            Debug.Log(v);
        }
    }
}
