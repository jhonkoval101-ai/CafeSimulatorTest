using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Resources")]
    public int Money { get; private set; } = 100;
    public float SoilPollution { get; private set; } = 0f; // 0-100%

    [Header("Time Settings")]
    [SerializeField] private float dayDuration = 180f; // 3 минуты
    public float CurrentDayTime { get; private set; } = 0f;
    public bool IsDayEnded { get; private set; } = false;

    [Header("Pollution Settings")]
    [SerializeField] private float autoDecayRate = 0.5f; // Загрязнение уменьшается со временем

    // События
    public event Action<int> OnMoneyChanged;
    public event Action<float> OnPollutionChanged;
    public event Action OnDayEnded;

    void Awake()
    {
        // Жёсткая инициализация синглтона
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (IsDayEnded) return;

        // Время дня
        CurrentDayTime += Time.deltaTime;
        if (CurrentDayTime >= dayDuration)
        {
            EndDay();
        }

        // Автоматическое уменьшение загрязнения
        SoilPollution -= autoDecayRate * Time.deltaTime;
        SoilPollution = Mathf.Clamp(SoilPollution, 0f, 100f);
    }

    // Деньги
    public void AddMoney(int amount)
    {
        Money += amount;
        OnMoneyChanged?.Invoke(Money);
        Debug.Log($"Money: {Money} (+{amount})");
    }

    public bool SpendMoney(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            OnMoneyChanged?.Invoke(Money);
            Debug.Log($"Money: {Money} (-{amount})");
            return true;
        }
        Debug.Log("Not enough money!");
        return false;
    }

    // Загрязнение
    public void AddPollution(float amount)
    {
        SoilPollution += amount;
        SoilPollution = Mathf.Clamp(SoilPollution, 0f, 100f);
        OnPollutionChanged?.Invoke(SoilPollution);
        Debug.Log($"Pollution: {SoilPollution:F1}% (+{amount})");
    }

    // Конец дня
    private void EndDay()
    {
        IsDayEnded = true;
        OnDayEnded?.Invoke();
        Debug.Log("Day Ended!");
        // Здесь будет показ экрана отчёта
    }

    // Новый день (для теста)
    public void StartNewDay()
    {
        CurrentDayTime = 0f;
        IsDayEnded = false;
    }
}
