using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ovrtest : MonoBehaviour
{
    // Right Controller의 트랜스폼 (XR Origin의 Right Controller 참조)
    public Transform ControllerTransform;

    // 매 프레임 업데이트
    protected virtual void Update()
    {
        BtnDown();
    }

    // 버튼 입력 처리
    void BtnDown()
    {
        // A 버튼 클릭 시
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            HandleClick();
            Debug.Log("A 버튼 클릭 감지"); // A 버튼 클릭 시 로그 추가
        }

        // B 버튼 클릭 시
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Debug.Log("B 버튼 클릭 감지"); // B 버튼 클릭 시 로그 추가
            HandleClick();
        }
    }

    // 클릭 처리 함수 (레이캐스트를 사용해 오브젝트 클릭 감지)
    protected virtual void HandleClick()
    {
        RaycastHit hit;

        // 컨트롤러의 위치에서 컨트롤러가 보는 방향으로 레이 발사
        Ray ray = new Ray(ControllerTransform.position, ControllerTransform.forward);

        // 레이를 시각적으로 그립니다 (길이를 1000으로 설정)
        Debug.DrawRay(ControllerTransform.position, ControllerTransform.forward * 3000, Color.red, 15.0f);

        // 레이가 충돌한 오브젝트 확인
        if (Physics.Raycast(ray, out hit, 1000)) // Raycast의 거리도 1000으로 설정
        {
            Debug.Log("Hit object: " + hit.collider.name); // 충돌한 오브젝트 이름 출력

            // 충돌한 오브젝트에 ObjectClick 컴포넌트가 있는지 확인
            ObjectClick objectClick = hit.collider.GetComponent<ObjectClick>();
            if (objectClick != null)
            {
                objectClick.LoadScene(); // 클릭된 오브젝트의 LoadScene 메서드 호출
            }
        }
    }
}
