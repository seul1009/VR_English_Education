using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBasket : MonoBehaviour
{
    public Transform BasketTransform; // 이동시킬 장바구니
    public Transform CheckoutPoint;  // 장바구니가 놓일 계산대 위치

    public void MoveToCheckout()
    {
        if (BasketTransform != null && CheckoutPoint != null)
        {
            // 부모 관계 해제
            BasketTransform.SetParent(null);

            // 장바구니를 계산대 위치로 이동
            BasketTransform.position = CheckoutPoint.position;
            BasketTransform.rotation = CheckoutPoint.rotation;

            Debug.Log("장바구니가 계산대로 이동했습니다.");
        }
        else
        {
            Debug.LogError("BasketTransform 또는 CheckoutPoint가 설정되지 않았습니다.");
        }
    }
}
