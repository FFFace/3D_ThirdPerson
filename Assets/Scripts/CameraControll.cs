using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [SerializeField]
    private Transform targetTR;

    [SerializeField]
    private float targetFollowSpeed;

    [SerializeField]
    private Camera mainCamera;

    /// <summary>
    /// 카메라 줌 인, 아웃 용
    /// </summary>

    //[SerializeField]
    //private float distance;

    //[SerializeField]
    //private float mouseWheelSensitive;

    [SerializeField]
    private float mouseAxisSensitive;

    private float mouseY = -20;

    private void Update()
    {
        CameraTargetFollow();
        CameraAxis();
        //CameraZoomInOut();
    }

    private void CameraTargetFollow()
    {
        Vector3 pos = targetTR.position;
        pos.y += 1.5f;
        float dis = Vector3.Distance(transform.position, pos);
        if (dis < 0.01f)
        {
            transform.position = pos;
            return;
        }

        transform.position = Vector3.Lerp(transform.position, pos, targetFollowSpeed * Time.deltaTime);
    }


    // 카메라 줌 인, 아웃 용

    //private void CameraZoomInOut()
    //{
    //    distance += Input.GetAxis("Mouse ScrollWheel") * mouseWheelSensitive * -1;
    //    distance = Mathf.Clamp(distance, 1.0f, 10.0f);

    //    Vector3 pos = -transform.forward;
    //    pos *= distance;

    //    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, transform.position + pos, 3 * Time.deltaTime);
    //}

    private void CameraAxis()
    {
        float axisX = targetTR.transform.rotation.eulerAngles.y;
        
        mouseY += Input.GetAxis("Mouse Y") * mouseAxisSensitive;
        mouseY = Mathf.Clamp(mouseY, -45, 45);

        Vector3 camAngle = new Vector3(-mouseY, axisX, 0);
        Quaternion angle = Quaternion.Euler(camAngle);
        transform.rotation = angle;
        //transform.rotation = Quaternion.Euler(camAngle);
    }
}
