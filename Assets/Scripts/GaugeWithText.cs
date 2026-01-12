using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GaugeWithText : MonoBehaviour
{
    [SerializeField] private HumanToRobotTransform _humanToRobotTransform;
    // private float _currentHumanToRobotTransform = 0;
    [SerializeField] private TextMeshProUGUI _percentText;
    [SerializeField] private Slider _gaugeSlider;
    [SerializeField] private GameObject _textBox;
    public bool _isPlaying = false;
    void Update()
    {
        _gaugeSlider.value = _humanToRobotTransform.transformValue * 0.01f;
        _percentText.text = Mathf.RoundToInt(_humanToRobotTransform.transformValue).ToString() + "%";

        if (!_isPlaying)
        {
            if (_humanToRobotTransform.transformValue <= 0)
            {
                _textBox.SetActive(true);
            }
            else
            {
                _isPlaying = true;
                _textBox.SetActive(false);
            }

        }

    }
    /// <summary>
    /// 모든 값 초기화
    /// </summary>
    public void AllInit()
    {
        _gaugeSlider.value = 0;
        _percentText.text = "0%";
        _textBox.SetActive(true);
        _isPlaying = false;
    }
}
