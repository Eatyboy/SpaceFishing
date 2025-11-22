using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    private InputSystem_Actions ctrl;

    public Button[] buttons;
    public int selectedButtonIndex = 0;
    public RectTransform selector;
    public Button selectedButton => buttons[selectedButtonIndex];

    private void Awake()
    {
        ctrl = new();
    }

    private void OnEnable()
    {
        ctrl.Enable();
        ctrl.Player.Button1.performed += ClickSelectedButton;
        ctrl.Player.Button2.performed += SelectNextButton;
    }

    private void OnDisable()
    {
        ctrl.Player.Button1.performed -= ClickSelectedButton;
        ctrl.Player.Button2.performed -= SelectNextButton;
        ctrl.Disable();
    }

    public void ClickSelectedButton(InputAction.CallbackContext ctx)
    {
        selectedButton.onClick.Invoke();
    }

    public void SelectNextButton(InputAction.CallbackContext ctx)
    {
        selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Length;
        selector.position = selectedButton.transform.position;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
