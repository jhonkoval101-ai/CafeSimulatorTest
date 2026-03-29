using UnityEngine;

public class CarryableObject : MonoBehaviour
{
    public enum TrashType
    {
        None,
        Food,
        Paper,
        Plastic,
        Mixed
    }

    [Header("Object Settings")]
    public TrashType type;
    public float decomposeTime = 5f;
    public float pollutionAmount = 10f;

    private bool _isHeld = false;
    private Transform _heldParent;

    // Добавь это свойство
    public bool IsHeld => _isHeld;

    public void PickUp(Transform holder)
    {
        _isHeld = true;
        _heldParent = holder;

        // Прикрепляем к игроку
        transform.SetParent(holder);
        transform.localPosition = new Vector3(0, 0.5f, 1);
        transform.localRotation = Quaternion.identity;

        // Отключаем физику
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        CancelInvoke(nameof(Decompose));
    }

    public void Drop(Vector3 dropPosition)
    {
        _isHeld = false;

        // Отцепляем от родителя
        transform.SetParent(null);
        transform.position = dropPosition;

        // Включаем физику
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // Добавляем небольшую силу вперед
            rb.AddForce(transform.forward * 2f + Vector3.up * 1f, ForceMode.Impulse);
        }

        // Запускаем разложение
        Invoke(nameof(Decompose), decomposeTime);
    }

    private void Decompose()
    {
        Debug.Log($"Object {gameObject.name} decomposed! Pollution: {pollutionAmount}");
        Destroy(gameObject);
    }
}