using UnityEngine;

public class Kitchen : MonoBehaviour
{
    [Header("Production Settings")]
    [SerializeField] private float productionTime = 5f; // Время создания еды
    [SerializeField] private int maxFoodOnCashier = 3; // Максимум еды на кассе
    [SerializeField] private float pollutionAmount = 5f; // Загрязнение при работе

    [Header("References")]
    [SerializeField] private Transform cashierSpawnPoint; // Точка создания еды
    [SerializeField] private GameObject foodPrefab; // Префаб еды

    [Header("Water Filter")]
    [SerializeField] private bool hasWaterFilter = false; // Есть ли фильтр

    private float _productionTimer = 0f;
    private int _foodCount = 0;

    void Update()
    {
        // Если есть фильтр — не добавляем загрязнение
        if (!hasWaterFilter)
        {
            _productionTimer += Time.deltaTime;

            if (_productionTimer >= productionTime)
            {
                ProduceFood();
                _productionTimer = 0f;
            }
        }
    }

    private void ProduceFood()
    {
        // Проверяем, есть ли место на кассе
        if (_foodCount >= maxFoodOnCashier)
        {
            Debug.Log("Kitchen: Cashier is full!");
            return;
        }

        // Создаём еду
        if (foodPrefab != null && cashierSpawnPoint != null)
        {
            GameObject food = Instantiate(foodPrefab, cashierSpawnPoint.position, Quaternion.identity);
            food.transform.SetParent(cashierSpawnPoint);
            _foodCount++;

            // Добавляем загрязнение
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddPollution(pollutionAmount);
            }

            Debug.Log($"Kitchen: Produced food! Total: {_foodCount}/{maxFoodOnCashier}");
        }
    }

    // Метод для уменьшения счётчика еды (вызывается при продаже)
    public void RemoveFood()
    {
        _foodCount = Mathf.Max(0, _foodCount - 1);
        Debug.Log($"Kitchen: Food sold. Remaining: {_foodCount}");
    }

    // Установка фильтра воды
    public void InstallWaterFilter()
    {
        hasWaterFilter = true;
        Debug.Log("Kitchen: Water filter installed! No more pollution from cooking.");
    }
}