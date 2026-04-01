using UnityEngine;

public class TrashBin : MonoBehaviour
{
    [Header("Bin Settings")]
    [SerializeField] private int maxCapacity = 5; // Максимум мусора в баке
    [SerializeField] private float trashDecayTime = 30f; // Время до создания мешка

    [Header("References")]
    [SerializeField] private Transform bagSpawnPoint; // Точка создания мешка
    [SerializeField] private GameObject trashBagPrefab; // Префаб мешка

    private int _currentTrash = 0;
    private float _decayTimer = 0f;

    void Update()
    {
        if (_currentTrash >= maxCapacity)
        {
            // Бак полон - создаём мешок
            _decayTimer += Time.deltaTime;

            if (_decayTimer >= trashDecayTime)
            {
                CreateTrashBag();
                _decayTimer = 0f;
                _currentTrash = 0;
            }
        }
    }

    // Игрок выбрасывает мусор в бак
    public void AddTrash()
    {
        if (_currentTrash < maxCapacity)
        {
            _currentTrash++;
            Debug.Log($"TrashBin: Added trash. {_currentTrash}/{maxCapacity}");

            if (_currentTrash >= maxCapacity)
            {
                Debug.Log("TrashBin: FULL! Creating trash bag soon...");
            }
        }
        else
        {
            Debug.Log("TrashBin: Already full!");
        }
    }

    private void CreateTrashBag()
    {
        if (trashBagPrefab != null && bagSpawnPoint != null)
        {
            GameObject bag = Instantiate(trashBagPrefab, bagSpawnPoint.position, Quaternion.identity);
            Debug.Log("TrashBin: Created trash bag!");
        }
    }

    // Визуализация
    void OnDrawGizmosSelected()
    {
        // Показываем ёмкость бака
        Gizmos.color = _currentTrash >= maxCapacity ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);

        // Индикатор заполненности
        float fillPercent = (float)_currentTrash / maxCapacity;
        Gizmos.color = Color.Lerp(Color.green, Color.red, fillPercent);
        Gizmos.DrawSphere(transform.position + Vector3.up * 2f, 0.3f);
    }
}
