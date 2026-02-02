using UnityEngine;
using UnityEngine.InputSystem;
public class AllGameInit : MonoBehaviour
{
    [SerializeField] private HumanToRobotTransform _humanToRobotTransform;
    [SerializeField] private GaugeWithText _gaugeWithText;
    [SerializeField] private SelectButtonHandler _selectButtonHandler;
    [SerializeField] private ControlManager _controlManager;

    [Header("자동 리셋 설정")]
    [SerializeField] private float autoResetTime = 30f;
    private float _idleTimer = 0f;
    private bool _isTimerRunning = false;

    void Update()
    {
        // 키보드 입력 감지 (1-5번 키)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame ||
                Keyboard.current.digit2Key.wasPressedThisFrame ||
                Keyboard.current.digit3Key.wasPressedThisFrame ||
                Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                ResetTimer();
            }

            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                AllInit();
            }
        }

        // 타이머 업데이트
        if (_isTimerRunning)
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= autoResetTime)
            {
                AllInit();
            }
        }
    }

    /// <summary>
    /// 타이머 시작 (외부에서 호출)
    /// </summary>
    public void StartTimer()
    {
        _idleTimer = 0f;
        _isTimerRunning = true;
    }

    /// <summary>
    /// 타이머 리셋 (입력이 있을 때)
    /// </summary>
    public void ResetTimer()
    {
        _idleTimer = 0f;
    }

    /// <summary>
    /// 타이머 정지
    /// </summary>
    public void StopTimer()
    {
        _isTimerRunning = false;
        _idleTimer = 0f;
    }
    /// <summary>
    /// All Reset 로직
    /// </summary>
    public void AllInit()
    {
        StopTimer();
        _humanToRobotTransform.AllInit();
        _gaugeWithText.AllInit();
        _selectButtonHandler.allInit();
        _controlManager?.ResetAll();
    }
}
