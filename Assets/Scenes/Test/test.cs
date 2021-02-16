using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    [SerializeField]
    private float maxHeight;

    private Vector3 startPos;
    private Vector3 middlePos;
    private Vector3 targetDir;
    private Vector3 middleDir;

    private void Start()
    {
        startPos = transform.position;
        targetDir = (target.transform.position - new Vector3(transform.position.x, target.transform.position.y, transform.position.z)).normalized;
        middleDir = (middlePos - new Vector3(transform.position.x, target.transform.position.y, transform.position.z)).normalized;
        middlePos = (target.transform.position + new Vector3(transform.position.x, target.transform.position.y, transform.position.z)) / 2;

    }

    private void Update()
    {
        targetDir = (target.transform.position - new Vector3(transform.position.x, target.transform.position.y, transform.position.z)).normalized;
        middleDir = (middlePos - new Vector3(transform.position.x, target.transform.position.y, transform.position.z)).normalized;

        float dot = Vector3.Dot(targetDir, middleDir);
        dot = Mathf.Sign(dot);

        float heightRate = (maxHeight - transform.position.y) / maxHeight;
        Debug.Log(heightRate);

        transform.Translate(Vector3.up * dot * 6 * heightRate * Time.deltaTime);
        transform.Translate(targetDir * 3 * Time.deltaTime);

        Debug.DrawRay(transform.position, targetDir * 5, Color.red);
        Debug.DrawRay(transform.position, middleDir * 5, Color.blue);
    }
}
