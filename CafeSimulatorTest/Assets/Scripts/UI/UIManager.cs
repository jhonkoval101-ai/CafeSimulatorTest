using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI pollutionText;
    [SerializeField] private Slider pollutionSlider;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor = Color.red;

    void Start()
    {
        // Подписка на события GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMoneyChanged += UpdateMoney;
            GameManager.Instance.OnPollutionChanged += UpdatePollution;

            // Инициализация начальных значений
            UpdateMoney(GameManager.Instance.Money);
            UpdatePollution(GameManager.Instance.SoilPollution);
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
    }

    void OnDestroy()
    {
        // Отписка от событий
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMoneyChanged -= UpdateMoney;
            GameManager.Instance.OnPollutionChanged -= UpdatePollution;
        }
    }

    private void UpdateMoney(int money)
    {
        if (moneyText != null)
        {
            moneyText.text = $"{money}$";
        }
    }

    private void UpdatePollution(float pollution)
    {
        if (pollutionText != null)
        {
            pollutionText.text = $"{pollution:F1}%";
        }

        if (pollutionSlider != null)
        {
            pollutionSlider.value = pollution;

            // Меняем цвет в зависимости от уровня загрязнения
            if (pollution < 50f)
            {
                pollutionSlider.GetComponentInChildren<Image>().color = normalColor;
            }
            else if (pollution < 75f)
            {
                pollutionSlider.GetComponentInChildren<Image>().color = warningColor;
            }
            else
            {
                pollutionSlider.GetComponentInChildren<Image>().color = dangerColor;
            }
        }
    }
}
