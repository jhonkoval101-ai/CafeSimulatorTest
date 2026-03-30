using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 5f;

    private CarryableObject _heldObject;
    private Camera _mainCamera;

    void Awake()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        // Подбор на E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_heldObject != null)
            {
                // Если держим предмет - бросаем
                DropObject();
            }
            else
            {
                // Иначе пытаемся подобрать
                TryPickUp();
            }
        }

        // Бросок на ПКМ
        if (Input.GetMouseButtonDown(1) && _heldObject != null)
        {
            DropObject();
        }
    }

    private void TryPickUp()
    {
        // Находим все объекты с CarryableObject
        CarryableObject[] allCarryables = FindObjectsByType<CarryableObject>(FindObjectsSortMode.None);

        // Сортируем по расстоянию вручную
        System.Array.Sort(allCarryables, (a, b) =>
        {
            float distA = Vector3.Distance(transform.position, a.transform.position);
            float distB = Vector3.Distance(transform.position, b.transform.position);
            return distA.CompareTo(distB);
        });

        Debug.Log($"Found {allCarryables.Length} carryable objects");

        foreach (var carryable in allCarryables)
        {
            float distance = Vector3.Distance(transform.position, carryable.transform.position);
            Debug.Log($"Object: {carryable.gameObject.name}, Distance: {distance:F1}, IsHeld: {carryable.IsHeld}");

            if (distance <= interactRange && !carryable.IsHeld)
            {
                PickUpObject(carryable);
                return;
            }
        }

        Debug.Log("No pickupable object nearby!");
    }

    private void PickUpObject(CarryableObject obj)
    {
        _heldObject = obj;
        obj.PickUp(transform);
        Debug.Log("Picked up: " + obj.gameObject.name);
    }

    private void DropObject()
    {
        if (_heldObject == null) return;

        Vector3 dropPoint = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;

        _heldObject.Drop(dropPoint);
        _heldObject = null;
        Debug.Log("Dropped object");
    }

    // Визуализация радиуса
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}