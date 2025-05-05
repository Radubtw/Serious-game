using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class moveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
