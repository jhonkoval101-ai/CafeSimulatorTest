using UnityEngine;

public class GardenBed : MonoBehaviour
{
    [Header("Growth Settings")]
    [SerializeField] private float growthTime = 10f; // Время роста овощей
    [SerializeField] private int maxVegetables = 5; // Максимум овощей на грядке
    [SerializeField] private float pollutionThreshold = 75f; // Макс. загрязнение для роста

    [Header("References")]
    [SerializeField] private Transform spawnPoint; // Точка создания овощей
    [SerializeField] private GameObject vegetablePrefab; // Префаб овощей

    [Header("Economy")]
    [SerializeField] private int sellPrice = 10; // Цена продажи

    private float _growthTimer = 0f;
    private int _vegetableCount = 0;
    private BoxCollider _bedCollider; // Добавили коллайдер для границ

    void Start()
    {
        // Пытаемся получить коллайдер
        _bedCollider = GetComponent<BoxCollider>();
    }

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

        if (vegetablePrefab != null)
        {
            //Используем рандомную позицию вместо spawnPoint
            Vector3 spawnPos = GetRandomPosition();

            Instantiate(vegetablePrefab, spawnPos, Quaternion.identity);
            _vegetableCount++;

            Debug.Log($"GardenBed: Grew vegetables! Total: {_vegetableCount}/{maxVegetables}");
        }
    }

    //случайная позиция внутри грядки
    private Vector3 GetRandomPosition()
    {
        Vector3 center = transform.position;
        float halfX, halfZ;

        // Если есть коллайдер — используем его границы
        if (_bedCollider != null)
        {
            halfX = _bedCollider.size.x * 0.5f * transform.lossyScale.x;
            halfZ = _bedCollider.size.z * 0.5f * transform.lossyScale.z;
        }
        else
        {
            // Иначе используем Scale
            halfX = transform.localScale.x * 0.5f;
            halfZ = transform.localScale.z * 0.5f;
        }

        // Генерируем случайные смещения
        float randomX = Random.Range(-halfX, halfX);
        float randomZ = Random.Range(-halfZ, halfZ);

        // Возвращаем позицию (Y чуть выше грядки)
        return center + transform.right * randomX + transform.forward * randomZ + Vector3.up * 0.6f;
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
        // Рисуем границы грядки
        if (TryGetComponent<BoxCollider>(out var col))
        {
            Gizmos.DrawWireCube(transform.position,
            new Vector3(col.size.x * transform.lossyScale.x,0.1f, col.size.z * transform.lossyScale.z));
        }
        else
        {
            Gizmos.DrawWireCube(transform.position,
                new Vector3(transform.localScale.x, 0.1f, transform.localScale.z));
        }


        // Индикатор загрязнения
        if (GameManager.Instance != null)
        {
            float pollution = GameManager.Instance.SoilPollution;
            Gizmos.color = pollution < pollutionThreshold ? Color.green : Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.up * 0.5f, 0.3f);
        }
    }
}
