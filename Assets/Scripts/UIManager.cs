using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public Slider hpBar;

    private void Update()
    {
        moneyText.text = $"$: {Player.instance.money}";
        hpBar.value = Player.instance.hp / Player.instance.maxHp;
    }
}
