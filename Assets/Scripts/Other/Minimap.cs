using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private Transform target;

    private void Start()
    {
        StartCoroutine(IEnumUpdate());   
    }

    private IEnumerator IEnumUpdate()
    {
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            Vector3 pos = target.position;
            pos.y = 20;
            transform.position = pos;
            yield return null;
        }
    }

    public void SetTarget(GameObject obj)
    {
        target = obj.transform;
    }
}
