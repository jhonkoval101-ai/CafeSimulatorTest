using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Префаб")]
    public GameObject customerPrefab;

    [Header("Точки маршрута")]
    public Transform roadPoint;
    public Transform cashierPoint;
    public Transform benchPoint;
    public Transform binPoint;

    [Header("Настройки")]
    public float spawnInterval = 10f;

    private float _timer = 0f;

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= spawnInterval)
        {
            SpawnCustomer();
            _timer = 0f;
        }
    }

    private void SpawnCustomer()
    {
        if (customerPrefab != null && roadPoint != null)
        {
            // Создаём покупателя
            GameObject customerObj = Instantiate(customerPrefab, roadPoint.position, Quaternion.identity);

            // 🔧 Получаем скрипт и передаём точки
            Customer customer = customerObj.GetComponent<Customer>();
            if (customer != null)
            {
                customer.Init(roadPoint, cashierPoint, benchPoint, binPoint);
            }

            Debug.Log("Spawner: Новый покупатель создан!");
        }
    }
}
