using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRMover : MonoBehaviour
{
    public float moveSpeed = 1.0f; // �̵� �ӵ�
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController�� �� ���� ������Ʈ�� �߰��Ǿ� ���� �ʽ��ϴ�.");
        }
    }

    void Update()
    {
        // �̵� ���� ��� (�Է¿� ����)
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // ī�޶� ���⿡ �°� ����
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = 0; // ���� ���� �̵� ����
        moveDirection.Normalize();

        // �̵�
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
}
