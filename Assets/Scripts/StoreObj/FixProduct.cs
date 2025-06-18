using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixProduct : MonoBehaviour
{
    public Transform ParentTransform;

    // ��ٱ��Ͽ� ��� ������Ʈ�� �̵��� ����
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� �±װ� "Product"���� Ȯ��
        if (collision.gameObject.CompareTag("Product"))
        {
            // Rigidbody ������Ʈ ��������
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            // Rigidbody�� �����ϴ� ���
            if (rb != null)
            {
                rb.isKinematic = true; // isKinematic Ȱ��ȭ

                collision.gameObject.transform.SetParent(ParentTransform); // �ڽ����� ����
            }
        }
    }
}
