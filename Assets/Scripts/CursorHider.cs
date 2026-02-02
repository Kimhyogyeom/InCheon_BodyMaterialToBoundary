using UnityEngine;

public class CursorHider : MonoBehaviour
{
    void Start()
    {
        // 커서 숨기기
        Cursor.visible = false;

        // 커서 잠금 (화면 밖으로 안 나가게)
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("[CursorHider] 커서 숨김 완료");
    }

    void Update()
    {
        // ESC 키로 커서 다시 보이게 (테스트/디버깅용)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("[CursorHider] 커서 표시 (ESC)");
        }
    }
}
