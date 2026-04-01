using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("Настройки движения")]
    [Tooltip("Скорость бега игрока")]
    public float moveSpeed = 5f;

    [Tooltip("Скорость поворота персонажа")]
    public float rotationSpeed = 10f;

    private CharacterController controller;
    private Camera mainCamera;

    void Start()
    {
        // Получаем компоненты
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("PlayerMove: Камера не найдена! Убедись, что на камере есть тег MainCamera.");
        }
    }

    void Update()
    {
        // 1. Считываем ввод (WASD или стрелки)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Если нет ввода — ничего не делаем
        if (Mathf.Abs(horizontal) < 0.01f && Mathf.Abs(vertical) < 0.01f)
        {
            return;
        }

        // 2. Вычисляем направление относительно камеры
        // Берём векторы камеры
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        // Убираем наклон по Y (чтобы игрок не летел вверх/вниз, если камера наклонена)
        camForward.y = 0f;
        camRight.y = 0f;

        // Нормализуем, чтобы длина векторов была 1
        camForward.Normalize();
        camRight.Normalize();

        // Складываем направления: Вперёд * Vertical + Вправо * Horizontal
        Vector3 moveDirection = camForward * vertical + camRight * horizontal;
        moveDirection.Normalize();

        // Вычисляем, движется ли игрок назад
        // Dot > 0 = вперёд, Dot < 0 = назад
        float dot = Vector3.Dot(moveDirection, transform.forward);

        // Вращаем только если движемся вперёд или вбок
        // Если dot < -0.5, значит движение назад — пропускаем вращение
        if (dot > -0.5f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 4. Двигаем игрока
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
}
