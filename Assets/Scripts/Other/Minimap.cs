using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private void Update()
    {
        Vector3 pos = target.position;
        pos.y = 20;
        transform.position = pos;
    }
}
