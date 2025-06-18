using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRMover : MonoBehaviour
{
    public float moveSpeed = 1.0f; // 이동 속도
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController가 이 게임 오브젝트에 추가되어 있지 않습니다.");
        }
    }

    void Update()
    {
        // 이동 방향 계산 (입력에 따라)
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // 카메라 방향에 맞게 조정
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = 0; // 수직 방향 이동 제거
        moveDirection.Normalize();

        // 이동
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
}
