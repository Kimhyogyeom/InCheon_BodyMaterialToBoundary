using UnityEngine;
using UnityEngine.UI;

public class ExitButtonHandler : MonoBehaviour
{
    [SerializeField] private AllGameInit _allGameInit;

    [SerializeField] private Button _exitButton;

    void Awake()
    {
        _exitButton.onClick.AddListener(OnCLickExitButton);
    }

    private void OnCLickExitButton()
    {
        _allGameInit.AllInit();
    }
}
