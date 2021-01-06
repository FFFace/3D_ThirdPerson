using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class ItemSphere : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private bool isMove;

    private void OnEnable()
    {
        StartCoroutine(Move());
        StartCoroutine(IEnumDistance());
    }

    private IEnumerator IEnumDistance()
    {
        Debug.Log("A");
        while (true)
        {
            yield return new WaitForSeconds(2f);
            isMove = true;
            
            float dis = Vector3.Distance(Character.instance.transform.position, transform.position);
            if (dis < 5f)
            {
                yield return new WaitForSeconds(0.2f);
                GetComponent<MeshRenderer>().enabled = false;
                isMove = false;
                yield return new WaitForSeconds(2.0f);
                GetComponent<MeshRenderer>().enabled = true;
                gameObject.SetActive(false);
                ItemList.instance.ItemSphereEnqueue(this);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Move()
    {
        while (!isMove)
        {
            transform.Translate(Vector3.up * 1.5f * Time.deltaTime);
            yield return null;
        }

        transform.LookAt(Character.instance.transform);
        Vector3 rot = transform.rotation.eulerAngles;
        rot += new Vector3(-Random.Range(10.0f, 20.0f), Random.Range(-15.0f, 15.0f), 0);
        transform.rotation = Quaternion.Euler(rot);

        while (isMove)
        {
            Vector3 pos = Character.instance.transform.position;
            pos.y += 1f;
            Vector3 normal = (pos - transform.position).normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(normal), speed * 0.5f * Time.deltaTime);

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            yield return null;
        }
    }
}
