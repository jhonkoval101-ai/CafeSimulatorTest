using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;

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
        // Используем Physics.OverlapSphere для поиска объектов рядом
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);

        foreach (var hit in hitColliders)
        {
            CarryableObject carryable = hit.GetComponent<CarryableObject>();
            if (carryable != null && !carryable.IsHeld)
            {
                // Проверяем, что объект виден камере (опционально)
                PickUpObject(carryable);
                return;
            }
        }

        Debug.Log("No pickupable object nearby");
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

        _heldObject.Drop();
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