using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Locomotion;
using UnityEngine;

public class ObjectToBasket : ovrtest
{
    public Transform TargetTransform; // 이동할 대상의 위치
    public Transform HeldObjectTransform; // 클릭하는 동안 상품이 존재할 위치
    public MoveBasket BasketMover;

    private GameObject selectedObject = null; // 현재 잡고 있는 오브젝트
    private bool isHoldingObject = false; // 트리거로 오브젝트를 잡고 있는 상태인지 확인

    protected new void Update()
    {
        HandleSecondaryIndexTrigger(); // 트리거 입력 처리
    }

    private void HandleSecondaryIndexTrigger()
    {
        // 트리거 버튼이 눌릴 때
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Debug.Log("트리거 버튼 클릭 감지");
            PickUpObject(); // 오브젝트 집기
        }

        // 트리거 버튼을 누르고 있는 동안
        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && isHoldingObject && selectedObject != null)
        {
            MoveWithController(selectedObject); // 잡은 오브젝트가 컨트롤러를 따라 움직임
        }

        // 트리거 버튼을 놓았을 때
        if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger) && isHoldingObject && selectedObject != null)
        {
            // ReleaseObject(); // 잡고 있던 오브젝트를 놓고 장바구니로 이동
            ReleaseObjectDrop(selectedObject);
        }
    }

    private void PickUpObject()
    {
        RaycastHit hit;
        Ray ray = new Ray(ControllerTransform.position, ControllerTransform.forward);

        if (selectedObject != null)
        {
            selectedObject = null;
        }
        else if (Physics.Raycast(ray, out hit, 1000))
        {
            Debug.Log("Hit object: " + hit.collider.name + "\n" + "Tag: " + hit.collider.tag);

            if (hit.collider.CompareTag("Product")) // Tag가 "Product"인 경우
            {
                selectedObject = hit.collider.gameObject; // 오브젝트 선택
                isHoldingObject = true; // 상태 업데이트
                Debug.Log($"{selectedObject.name}을(를) 잡았습니다.");
            }
            else if (hit.collider.CompareTag("Clerk")) // Tag가 "Clerk"인 경우
            {
                Debug.Log("계산하기");
                if (BasketMover != null)
                {
                    BasketMover.MoveToCheckout(); // 장바구니 이동 호출
                }
            }
            else
            {
                Debug.Log("상품이 아닙니다.");
            }
        }
        if (Physics.Raycast(ray, out hit, 1000))
        {
            Debug.Log("Hit object: " + hit.collider.name + "\n" + "Tag: " + hit.collider.tag);

            if (hit.collider.CompareTag("Product")) // Tag가 "Product"인 경우
            {
                selectedObject = hit.collider.gameObject; // 오브젝트 선택
                isHoldingObject = true; // 상태 업데이트
                Debug.Log($"{selectedObject.name}을(를) 잡았습니다.");
            }
            else if (hit.collider.CompareTag("Clerk")) // Tag가 "Clerk"인 경우
            {
                Debug.Log("계산하기");
                if (BasketMover != null)
                {
                    BasketMover.MoveToCheckout(); // 장바구니 이동 호출
                }
            }
            else
            {
                Debug.Log("상품이 아닙니다.");
            }
        }
    }

    // 오브젝트를 컨트롤러와 함께 이동
    private void MoveWithController(GameObject obj)
    {
        if (isHoldingObject != false)
        {
            obj.transform.SetParent(HeldObjectTransform); // HeldObjectTransform의 자식 요소로 설정
            obj.transform.localPosition = Vector3.zero; // 상대적 위치 초기화
                                                        // obj.transform.localRotation = Quaternion.identity; // 상대적 회전 초기화

            // 움직이지 못하도록 Rigidbody 비활성화
            Rigidbody rb = obj.GetComponent<Rigidbody>(); // 오브젝트가 Rigidbody 컴포넌트를 소유 중인지 확인
            if (rb != null) // 소유 중이면
            {
                rb.isKinematic = true; // 물리 상호작용 비활성화
            }
        }
    }

    // 잡고 있던 오브젝트를 놓고 장바구니로 이동
    // private void ReleaseObject()
    // {
    //     if (selectedObject != null)
    //     {
    //         Debug.Log($"{selectedObject.name}을(를) 장바구니에 담았습니다.");
    //         MoveToBasket(selectedObject); // 장바구니로 이동 처리
    //         selectedObject = null; // 선택 초기화
    //         isHoldingObject = false; // 상태 초기화
    //     }
    // }

    // 트리거를 놓으면 바닥으로 떨어짐
    private void ReleaseObjectDrop(GameObject obj)
    {
        selectedObject.transform.SetParent(null); // 물건이 떨어질 수 있도록 부모관계 해제

        if (selectedObject != null) // selectedObject가 존재하는 경우
        {
            // 물건을 놓으면 바닥으로 떨어지도록 Rigidbody 추가
            Rigidbody rb = obj.GetComponent<Rigidbody>(); // 오브젝트가 Rigidbody 컴포넌트를 소유 중인지 확인
            if (rb == null) // Rigidbody 컴포넌트가 없으면
            {
                obj.AddComponent<Rigidbody>(); // Rigidbody 추가
                rb.useGravity = true; // 중력 활성화
            }
            else // Rigidbody 컴포넌트가 있으면
            {
                rb.isKinematic = false; // 물리 상호작용 활성화
            }
            Debug.Log($"{obj.name}을(를) 장바구니에 담았습니다.");
        }
        StartCoroutine(DelayedReset());
    }

    // // 트리거를 놓으면 장바구니로 이동
    // private void MoveToBasket(GameObject obj)
    // {
    //     obj.transform.SetParent(TargetTransform); // TargetTransform의 자식 요소로 설정
    //     obj.transform.localPosition = Vector3.zero; // 상대적 위치 초기화
    //     obj.transform.localRotation = Quaternion.identity; // 상대적 회전 초기화

    //     // 움직이지 못하도록 Rigidbody 비활성화
    //     Rigidbody rb = obj.GetComponent<Rigidbody>(); // 오브젝트가 Rigidbody 컴포넌트를 소유 중인지 확인
    //     if (rb != null)
    //     {
    //         rb.isKinematic = true;
    //     }

    //     Debug.Log($"{obj.name}이(가) {TargetTransform.name}로 이동했습니다.");
    // }

    private IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        selectedObject = null;
        isHoldingObject = false;
    }

}
