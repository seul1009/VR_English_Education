using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ovrtest : MonoBehaviour
{
    // Right Controller�� Ʈ������ (XR Origin�� Right Controller ����)
    public Transform ControllerTransform;

    // �� ������ ������Ʈ
    protected virtual void Update()
    {
        BtnDown();
    }

    // ��ư �Է� ó��
    void BtnDown()
    {
        // A ��ư Ŭ�� ��
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            HandleClick();
            Debug.Log("A ��ư Ŭ�� ����"); // A ��ư Ŭ�� �� �α� �߰�
        }

        // B ��ư Ŭ�� ��
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Debug.Log("B ��ư Ŭ�� ����"); // B ��ư Ŭ�� �� �α� �߰�
            HandleClick();
        }
    }

    // Ŭ�� ó�� �Լ� (����ĳ��Ʈ�� ����� ������Ʈ Ŭ�� ����)
    protected virtual void HandleClick()
    {
        RaycastHit hit;

        // ��Ʈ�ѷ��� ��ġ���� ��Ʈ�ѷ��� ���� �������� ���� �߻�
        Ray ray = new Ray(ControllerTransform.position, ControllerTransform.forward);

        // ���̸� �ð������� �׸��ϴ� (���̸� 1000���� ����)
        Debug.DrawRay(ControllerTransform.position, ControllerTransform.forward * 3000, Color.red, 15.0f);

        // ���̰� �浹�� ������Ʈ Ȯ��
        if (Physics.Raycast(ray, out hit, 1000)) // Raycast�� �Ÿ��� 1000���� ����
        {
            Debug.Log("Hit object: " + hit.collider.name); // �浹�� ������Ʈ �̸� ���

            // �浹�� ������Ʈ�� ObjectClick ������Ʈ�� �ִ��� Ȯ��
            ObjectClick objectClick = hit.collider.GetComponent<ObjectClick>();
            if (objectClick != null)
            {
                objectClick.LoadScene(); // Ŭ���� ������Ʈ�� LoadScene �޼��� ȣ��
            }
        }
    }
}
