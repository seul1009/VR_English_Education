using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRCamContoller : MonoBehaviour
{
    public float mouseSensitivity = 600f; // 마우스감도

    private float MouseX; // 좌우 회전 축 
    private float MouseY; // 상하 회전 축

    void Update()
    {
        Rotate();
    }
    // 현재 마우스로 상하 회전 입력 시 콜라이더 전체가 돌아가게 되어 플레이어의 시점과 바닥이 가까워지는 문제 발견.
    // 오큘러스 장비에서도 해당 문제가 발생하는지 확인 필요
    private void Rotate()
    {
        if (Input.GetMouseButton(1))
        {
            MouseX += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime; // 입력값과 감도를 계산하여 좌우 회전
            // MouseY -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime; // 입력값과 감도를 계산하여 상하 회전
        }

        // MouseY = Mathf.Clamp(MouseY, -80f, 80f); // 상하 회전시 각도 제한
        transform.localRotation = Quaternion.Euler(MouseY, MouseX, 0f);// Quaternion.Euler(X축, Y축, Z축)
    }
}