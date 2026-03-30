using UnityEngine;

public class GardenBed : MonoBehaviour
{
    [Header("Growth Settings")]
    [SerializeField] private float growthTime = 10f; // Время роста овощей
    [SerializeField] private int maxVegetables = 2; // Максимум овощей на грядке
    [SerializeField] private float pollutionThreshold = 75f; // Макс. загрязнение для роста

    [Header("References")]
    [SerializeField] private Transform spawnPoint; // Точка создания овощей
    [SerializeField] private GameObject vegetablePrefab; // Префаб овощей

    [Header("Economy")]
    [SerializeField] private int sellPrice = 10; // Цена продажи

    private float _growthTimer = 0f;
    private int _vegetableCount = 0;

    void Update()
    {
        // Проверяем уровень загрязнения
        if (GameManager.Instance == null) return;

        float currentPollution = GameManager.Instance.SoilPollution;

        // Овощи растут только при низком загрязнении
        if (currentPollution < pollutionThreshold)
        {
            _growthTimer += Time.deltaTime;

            if (_growthTimer >= growthTime)
            {
                GrowVegetable();
                _growthTimer = 0f;
            }
        }
        else
        {
            // Загрязнение слишком высокое — рост прекращается
            if (_growthTimer > 0)
            {
                Debug.Log("GardenBed: Pollution too high! Growth stopped.");
                _growthTimer = 0f;
            }
        }
    }

    private void GrowVegetable()
    {
        if (_vegetableCount >= maxVegetables)
        {
            Debug.Log("GardenBed: No space for more vegetables!");
            return;
        }

        if (vegetablePrefab != null && spawnPoint != null)
        {
            GameObject vegetable = Instantiate(vegetablePrefab, spawnPoint.position, Quaternion.identity);
            // НЕ делаем дочерним!
            // vegetable.transform.SetParent(spawnPoint);

            _vegetableCount++;

            Debug.Log($"GardenBed: Grew vegetables! Total: {_vegetableCount}/{maxVegetables}");
        }
    }

    // Метод для уменьшения счётчика (вызывается при продаже)
    public void RemoveVegetable()
    {
        _vegetableCount = Mathf.Max(0, _vegetableCount - 1);
        Debug.Log($"GardenBed: Vegetables sold. Remaining: {_vegetableCount}");
    }

    // Продажа овощей (вызывается из скрипта продажи)
    public int GetSellPrice()
    {
        return sellPrice;
    }

    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {
        // Показываем радиус грядки
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2f);

        // Индикатор загрязнения
        if (GameManager.Instance != null)
        {
            float pollution = GameManager.Instance.SoilPollution;
            Gizmos.color = pollution < pollutionThreshold ? Color.green : Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.up * 0.5f, 0.3f);
        }
    }
}
