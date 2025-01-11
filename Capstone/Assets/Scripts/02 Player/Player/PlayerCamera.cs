using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivity = 10f;       // 마우스 감도
    [SerializeField]
    private PlayerController player;            // 플레이어    
    
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    public void Rotate()
    {
        // 추후에 문제 발생 시 Time.deltaTime 고려
        // float mouseX = player.MouseDelta.x * mouseSensitivity * Time.deltaTime;
        // float mouseY = player.MouseDelta.y * mouseSensitivity * Time.deltaTime;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        horizontalRotation += mouseX;
        
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);                 // 상하 회전을 -90도에서 90도로 제한
        
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);        // 카메라 상하 회전 적용 (X축 회전)
        
        player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);    // 플레이어 좌우 회전

        player.leftArm.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        player.rightArm.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // player.transform.Rotate(Vector3.up * mouseX);
        // transform.Rotate(Vector3.right * -mouseY);
        //
        // player.leftArm.transform.Rotate(Vector3.up * mouseX);
        // player.rightArm.transform.Rotate(Vector3.up * mouseX);
    }
}
