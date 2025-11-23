using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
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

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }
    private IEnumerator StartRoutine()
    {
        yield return new WaitForEndOfFrame();
        SelectButton(0);
    }

    public void SelectButton(int index)
    {
        selectedButtonIndex = index;
        selector.position = selectedButton.transform.position;
        selector.sizeDelta = 2.0f * new Vector2(200 + selectedButton.GetComponent<RectTransform>().rect.width, 100);
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.uiHover);
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
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.uiClick);
        selectedButton.onClick.Invoke();
    }

    public void SelectNextButton(InputAction.CallbackContext ctx)
    {
        SelectButton((selectedButtonIndex + 1) % buttons.Length);
    }

    public void RestartGame()
    {
        StartCoroutine(RestartGameCoroutine());
    }

    private IEnumerator RestartGameCoroutine()
    {
        yield return Fader.instance.FadeOut();
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        StartCoroutine(QuitGameCoroutine());
    }

    private IEnumerator QuitGameCoroutine()
    {
        yield return Fader.instance.FadeOut();

        SceneManager.LoadScene("TitleScreen");
    }
}
