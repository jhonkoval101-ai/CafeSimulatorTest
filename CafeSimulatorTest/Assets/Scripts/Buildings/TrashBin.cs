using UnityEngine;

public class TrashBin : MonoBehaviour
{
    // Тип бака: Обычный или Сортировочный
    public enum BinType { Normal, Sorted }

    [Header("Тип бака")]
    [SerializeField] private BinType binType = BinType.Normal;

    [Header("Bin Settings")]
    [SerializeField] private int maxCapacity = 5; // Максимум мусора в баке
    [SerializeField] private float trashDecayTime = 30f; // Время до создания мешка

    [Header("References")]
    [SerializeField] private Transform bagSpawnPoint; // Точка создания мешка

    [Header("Префабы мешков")]
    [SerializeField] private GameObject blackBagPrefab;   // Для обычного бака
    [SerializeField] private GameObject foodBagPrefab;    // Для сортировки (пищевые)
    [SerializeField] private GameObject paperBagPrefab;   // Для сортировки (бумага)
    [SerializeField] private GameObject plasticBagPrefab; // Для сортировки (пластик)

    private int _currentTrash = 0;
    private float _decayTimer = 0f;

    void Update()
    {
        if (_currentTrash >= maxCapacity)
        {
            // Бак полон - запускаем таймер
            _decayTimer += Time.deltaTime;

            if (_decayTimer >= trashDecayTime)
            {
                CreateTrashBag();
                _decayTimer = 0f;
                _currentTrash = 0;
            }
        }
    }

    // Игрок или покупатель выбрасывает мусор в бак
    public void AddTrash()
    {
        if (_currentTrash < maxCapacity)
        {
            _currentTrash++;
            Debug.Log($"[{name}] Trash added: {_currentTrash}/{maxCapacity}");

            if (_currentTrash >= maxCapacity)
            {
                Debug.Log($"[{name}] FULL! Creating bag in {trashDecayTime}s...");
            }
        }
        else
        {
            Debug.Log($"[{name}] Already full! Wait for bag.");
        }
    }

    private void CreateTrashBag()
    {
        if (bagSpawnPoint == null)
        {
            Debug.LogError("TrashBin: bagSpawnPoint not assigned!");
            return;
        }

        GameObject prefab = GetBagPrefab();

        if (prefab != null)
        {
            Instantiate(prefab, bagSpawnPoint.position, Quaternion.identity);
            Debug.Log($"[{name}] Created bag: {prefab.name}");
        }
        else
        {
            Debug.LogWarning("TrashBin: Bag prefab not assigned for this type!");
        }
    }

    // Выбор префаба в зависимости от типа бака
    private GameObject GetBagPrefab()
    {
        if (binType == BinType.Normal)
        {
            return blackBagPrefab;
        }
        else
        {
            // Сортировочный бак: случайный цветной мешок
            int rand = Random.Range(0, 3);
            if (rand == 0) return foodBagPrefab;
            if (rand == 1) return paperBagPrefab;
            return plasticBagPrefab;
        }
    }

    // Визуализация
    void OnDrawGizmosSelected()
    {
        // Цвет зависит от типа бака
        Gizmos.color = (binType == BinType.Sorted) ? Color.cyan : Color.yellow;

        // Показываем ёмкость бака
        Gizmos.DrawWireSphere(transform.position, 1.5f);

        // Линия к точке спавна
        if (bagSpawnPoint != null)
        {
            Gizmos.DrawLine(transform.position, bagSpawnPoint.position);
            Gizmos.DrawWireSphere(bagSpawnPoint.position, 0.3f);
        }

        // Индикатор заполненности
        float fillPercent = (float)_currentTrash / maxCapacity;
        Gizmos.color = Color.Lerp(Color.green, Color.red, fillPercent);
        Gizmos.DrawSphere(transform.position + Vector3.up * 2f, 0.3f);
    }
}