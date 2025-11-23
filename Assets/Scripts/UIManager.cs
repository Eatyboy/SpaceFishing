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
    public RectTransform costPopupOrigin;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void Update()
    {
        if (Player.instance != null)
        {
            moneyText.text = $"${Player.instance.money}";
            hpBar.value = Player.instance.hp / Player.instance.maxHp;
        }
    }

    public void TextPopup(int money)
    {
        Instantiate(moneyPopupPrefab, transform).Initialize(money, moneyPopupOrigin.position);
    }

    public void CostPopup(int cost)
    {
        Vector3 popupPosition = costPopupOrigin != null ? costPopupOrigin.position : moneyPopupOrigin.position + Vector3.down * 50f;
        MoneyPopup popup = Instantiate(moneyPopupPrefab, transform);
        popup.Initialize(-cost, popupPosition);
    }
}
