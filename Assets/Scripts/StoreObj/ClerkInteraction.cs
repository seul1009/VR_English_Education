using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClerkInteraction : MonoBehaviour
{
    public Transform ControllerTransform; // Right Controller 트랜스폼
    public MoveBasket BasketMover; // 장바구니 이동 스크립트 연결

    void Update()
    {
        HandleSecondaryIndexTrigger();
    }

    // PrimaryIndexTrigger 입력 처리
    private void HandleSecondaryIndexTrigger()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) // 트리거 버튼이 눌리는 순간 처리
        {
            Debug.Log("트리거 버튼 클릭 감지"); // 로그 출력
            HandleClick();
        }
    }

    // 클릭 처리
    private void HandleClick()
    {
        RaycastHit hit;

        // 컨트롤러 방향으로 레이 발사
        Ray ray = new Ray(ControllerTransform.position, ControllerTransform.forward);

        if (Physics.Raycast(ray, out hit, 1000)) // 1000 단위의 거리로 Raycast
        {
            Debug.Log("Hit object: " + hit.collider.name + "\n" + "Tag: " + hit.collider.tag);

            // 점원 오브젝트와 충돌했는지 확인
            if (hit.collider.CompareTag("Clerk"))
            {
                Debug.Log("계산하기");
                if (BasketMover != null)
                {
                    BasketMover.MoveToCheckout(); // 장바구니 이동 호출
                }
            }
        }
    }
}
