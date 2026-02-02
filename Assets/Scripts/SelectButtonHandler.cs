using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SelectButtonHandler : MonoBehaviour
{
    [SerializeField] private HumanToRobotTransform _humanToRobotTransform;
    [SerializeField] private AllGameInit _gameInit;

    [SerializeField] private TextMeshProUGUI _selectText;
    [SerializeField] private Image _selectImage;
    [SerializeField] private GameObject _endBox;
    [SerializeField] private TextMeshProUGUI _endText;

    void Update()
    {
        if (_endBox.activeSelf) return;

        // 사람으로 버튼
        if (Keyboard.current != null &&
            Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            OnClickSelectButtonUp();
        }
        // 로봇으로 버튼
        if (Keyboard.current != null &&
            Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            OnClickSelectButtonDown();
        }
        // 사람/로봇 선택 버튼
        if (Keyboard.current != null &&
             Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            OnClickSelectCheck();
        }
        // 마지막 확정 버튼
        if (Keyboard.current != null &&
             Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            OnClickSelectEnd();
        }
    }

    /// <summary>
    /// 레버 업 (사람)
    /// </summary>
    public void OnClickSelectButtonUp() => _selectText.text = "사람";
    /// <summary>
    /// 레버 다운 (로봇)
    /// </summary>
    public void OnClickSelectButtonDown() => _selectText.text = "로봇";
    /// <summary>
    /// 확정 버튼 (배경)
    /// </summary>
    public void OnClickSelectCheck() => _selectImage.enabled = true;
    public void OnClickSelectEnd()
    {
        _endBox.SetActive(true);
        int value = (int)_humanToRobotTransform.transformValue;

        string _humanValue = (100 - value).ToString();
        string _robotValue = (value).ToString();
        _endText.text = $"당신은\n사람 {_humanValue}% 기계 {_robotValue}%를\n{_selectText.text} 이라고 선택했습니다.";

        // 30초 타이머 시작
        _gameInit.StartTimer();
    }

    /// <summary>
    /// 리셋 버튼
    /// </summary>
    public void allInit()
    {
        _selectText.text = "사람";
        _selectImage.enabled = false;
        _endBox.SetActive(false);
    }
}
