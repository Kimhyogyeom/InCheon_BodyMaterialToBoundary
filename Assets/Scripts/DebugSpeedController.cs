using UnityEngine;

public class DebugSpeedController : MonoBehaviour
{
    [SerializeField] private bool debugMode;

    private void OnValidate()
    {
        ApplySpeed();
    }

    private void Awake()
    {
        ApplySpeed();
    }

    private void ApplySpeed()
    {
        Time.timeScale = debugMode ? 1f / 3f : 1f;
    }
}
