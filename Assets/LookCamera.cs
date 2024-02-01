using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LookCamera : NetworkBehaviour
{
    private void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
    }
}
