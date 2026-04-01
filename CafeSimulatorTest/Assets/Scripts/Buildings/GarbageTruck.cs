using UnityEngine;
using System.Collections.Generic;

public class GarbageTruck : MonoBehaviour
{
    // ==========================================
    // НАСТРОЙКИ
    // ==========================================

    [Header("Маршрут")]
    [Tooltip("Точки маршрута вдоль дороги")]
    public Transform[] checkpoints;

    [Header("Движение")]
    [Tooltip("Скорость грузовика")]
    public float speed = 2f;

    [Tooltip("Пауза на конечных точках (сек)")]
    public float waitTime = 3f;

    // ==========================================
    // ПЕРЕМЕННЫЕ
    // ==========================================

    private int currentIndex = 0;
    private bool isGoingForward = true;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    // ==========================================
    // ОБНОВЛЕНИЕ
    // ==========================================

    void Update()
    {
        // Защита: если точек нет, ничего не делаем
        if (checkpoints == null || checkpoints.Length == 0) return;

        if (isWaiting)
        {
            HandleWaiting();
            return;
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Transform target = checkpoints[currentIndex];
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (distance > 0.5f)
        {
            // Поворот
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
            }

            // Движение
            transform.position += direction.normalized * speed * Time.deltaTime;
        }
        else
        {
            OnArrivedAtCheckpoint();
        }
    }

    private void OnArrivedAtCheckpoint()
    {
        transform.position = checkpoints[currentIndex].position;
        isWaiting = true;
        waitTimer = 0f;

        Debug.Log($"Грузовик: Прибыл на точку {currentIndex}. Сбор мусора...");

        // Запускаем сбор и штрафы
        CollectTrashAndCharge();
    }

    private void HandleWaiting()
    {
        waitTimer += Time.deltaTime;
        if (waitTimer >= waitTime)
        {
            isWaiting = false;
            SwitchToNextCheckpoint();
        }
    }

    private void SwitchToNextCheckpoint()
    {
        if (isGoingForward)
        {
            currentIndex++;
            if (currentIndex >= checkpoints.Length - 1)
            {
                currentIndex = checkpoints.Length - 1;
                isGoingForward = false;
                Debug.Log("Грузовик: Конец маршрута. Еду обратно.");
            }
        }
        else
        {
            currentIndex--;
            if (currentIndex <= 0)
            {
                currentIndex = 0;
                isGoingForward = true;
                Debug.Log("Грузовик: Начало маршрута. Еду вперёд.");
            }
        }
    }

    // ==========================================
    // СБОР МУСОРА И ШТРАФЫ
    // ==========================================

    private void CollectTrashAndCharge()
    {
        // Используем надёжный поиск для Unity 6
        CarryableObject[] items = FindObjectsByType<CarryableObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        if (items.Length == 0) return;

        int totalPenalty = 0;
        List<GameObject> itemsToRemove = new List<GameObject>();

        foreach (var item in items)
        {
            // Проверяем, есть ли штраф у предмета
            if (item.disposalPenalty > 0)
            {
                totalPenalty += item.disposalPenalty;
                itemsToRemove.Add(item.gameObject);
                Debug.Log($"Грузовик: Забрал {item.name}. Штраф: -{item.disposalPenalty}$");
            }
        }

        // Удаляем мешки
        foreach (var obj in itemsToRemove)
        {
            Destroy(obj);
        }

        // Снимаем деньги
        if (totalPenalty > 0 && GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(-totalPenalty);
            Debug.Log($"Грузовик: Итого списано {totalPenalty}$ за вывоз.");
        }
    }

    // ==========================================
    // ВИЗУАЛИЗАЦИЯ
    // ==========================================

    void OnDrawGizmos()
    {
        if (checkpoints == null || checkpoints.Length < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < checkpoints.Length - 1; i++)
        {
            Gizmos.DrawLine(checkpoints[i].position, checkpoints[i + 1].position);
        }

        Gizmos.color = Color.green;
        foreach (var point in checkpoints)
        {
            if (point != null) Gizmos.DrawWireSphere(point.position, 0.5f);
        }
    }
}