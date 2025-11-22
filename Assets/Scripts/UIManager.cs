using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI moneyText;
    public Slider hpBar;
    public MoneyPopup moneyPopupPrefab;
    public RectTransform moneyPopupOrigin;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void Update()
    {
        moneyText.text = $"${Player.instance.money}";
        hpBar.value = Player.instance.hp / Player.instance.maxHp;
    }

    public void TextPopup(int money)
    {
        Instantiate(moneyPopupPrefab, transform).Initialize(money, moneyPopupOrigin.position);
    }
}
