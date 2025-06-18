using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixProduct : MonoBehaviour
{
    public Transform ParentTransform;

    // 장바구니에 담긴 오브젝트의 이동을 제한
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트의 태그가 "Product"인지 확인
        if (collision.gameObject.CompareTag("Product"))
        {
            // Rigidbody 컴포넌트 가져오기
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            // Rigidbody가 존재하는 경우
            if (rb != null)
            {
                rb.isKinematic = true; // isKinematic 활성화

                collision.gameObject.transform.SetParent(ParentTransform); // 자식으로 설정
            }
        }
    }
}
