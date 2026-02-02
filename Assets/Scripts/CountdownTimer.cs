using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    private int _remainingSeconds;
    private float _elapsed;

    private const int StartSeconds = 10;
    private const string Suffix = "초 후 원래 화면으로..";

    private void OnEnable()
    {
        _remainingSeconds = StartSeconds;
        _elapsed = 0f;
        UpdateText();
    }

    private void Update()
    {
        if (_remainingSeconds <= 0)
            return;

        _elapsed += Time.deltaTime;

        if (_elapsed >= 1f)
        {
            _elapsed -= 1f;
            _remainingSeconds--;

            if (_remainingSeconds > 0)
                UpdateText();
        }
    }

    private void UpdateText()
    {
        _timerText.text = $"{_remainingSeconds}{Suffix}";
    }
}
