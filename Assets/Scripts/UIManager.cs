using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    private void Update()
    {
        moneyText.text = $"$:{Player.instance.money}";
    }
}
