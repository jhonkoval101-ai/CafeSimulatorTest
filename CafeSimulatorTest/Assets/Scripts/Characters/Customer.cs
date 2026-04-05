using UnityEngine;
using System.Collections;

public class Customer : MonoBehaviour
{
    [Header("Настройки")]
    public float moveSpeed = 2f;
    public float waitTime = 5f;
    public int foodPrice = 15;
    public float dropChance = 0.01f;

    [Header("Префабы")]
    public GameObject foodPrefab;
    public GameObject smallTrashPrefab;

    // 🔧 Точки теперь приватные, получаем через Init
    private Transform roadPoint;
    private Transform cashierPoint;
    private Transform benchPoint;
    private Transform binPoint;

    // Состояния
    private enum State
    {
        MoveToCashier, Buy, DecidePath, MoveToBench, WaitAtBench,
        MoveToBin, Dispose, MoveToRoad, DecideLoop, Destroy
    }

    private State state
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                Debug.Log($"Customer State: {_state} -> {value}");
                _state = value;
            }
        }
    }

    private State _state = State.MoveToCashier;
    private Transform _target;
    private bool _hasFood = false;
    private bool _binExists = false;
    private TrashBin _trashBin;

    // 🔧 Метод инициализации (вызывается спавнером)
    public void Init(Transform road, Transform cashier, Transform bench, Transform bin)
    {
        roadPoint = road;
        cashierPoint = cashier;
        benchPoint = bench;
        binPoint = bin;
        _target = cashierPoint;

        Debug.Log($"Customer Init: Road={road != null}, Cashier={cashier != null}, Bench={bench != null}, Bin={bin != null}");
    }

    void Start()
    {
        // Проверяем наличие бака
        _trashBin = FindAnyObjectByType<TrashBin>();
        _binExists = _trashBin != null;

        // Защита: если Init не вызван
        if (cashierPoint == null)
        {
            Debug.LogError("Customer: Точки не назначены! Вызови Init() при спавне.");
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.MoveToCashier:
                if (MoveTowards(_target)) state = State.Buy;
                break;

            case State.Buy:
                PerformBuy();
                break;

            case State.DecidePath:
                DecideNextPath();
                break;

            case State.MoveToBench:
                if (MoveTowards(_target))
                {
                    Debug.Log("Customer: Пришёл на скамейку, начинаю ждать");
                    _state = State.WaitAtBench;
                    StartCoroutine(WaitThen(() =>
                    {
                        Debug.Log("Customer: Закончил ждать на скамейке");
                        _state = State.DecidePath;
                    }));
                }
                CheckRandomDrop();
                break;

            case State.WaitAtBench:
                CheckRandomDrop();
                break;

            case State.MoveToBin:
                if (MoveTowards(_target))
                {
                    state = State.Dispose;
                }
                CheckRandomDrop();
                break;

            case State.Dispose:
                PerformDispose();
                break;

            case State.MoveToRoad:
                if (MoveTowards(_target))
                {
                    state = State.DecideLoop;
                }
                CheckRandomDrop();
                break;

            case State.DecideLoop:
                DecideLoopOrExit();
                break;

            case State.Destroy:
                Destroy(gameObject);
                break;
        }
    }

    private bool MoveTowards(Transform target)
    {
        // 1. Если цели нет — считаем что прибыли
        if (target == null)
        {
            Debug.LogWarning($"Customer: Target is NULL in state {_state}");
            return true;
        }

        // 2. Вычисляем направление и расстояние
        Vector3 dir = (target.position - transform.position);
        dir.y = 0; // Игнорируем высоту
        float distance = dir.magnitude;

        // 3. Лог для отладки (раз в секунду)
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"POS: Player={transform.position}, Target={target.position} ({target.name}), Dist={distance:F2}");
        }

        // 4. Проверка прибытия
        if (distance < 0.5f)
        {
            Debug.Log($"ARRIVED: {target.name}");
            return true;
        }

        // 5. Движение к цели
        transform.position += dir.normalized * moveSpeed * Time.deltaTime;

        // 6. Поворот в сторону движения
        if (dir.magnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5f * Time.deltaTime);
        }

        // 7. Ещё не прибыли
        return false;
    }

    private void PerformBuy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(foodPrice);
        }

        if (foodPrefab != null && !_hasFood)
        {
            GameObject food = Instantiate(foodPrefab, transform.position + Vector3.up * 1.2f, Quaternion.identity);
            food.transform.SetParent(transform);
            _hasFood = true;
        }

        _state = State.DecidePath;
    }

    private void DecideNextPath()
    {
        float pollution = GameManager.Instance != null ? GameManager.Instance.SoilPollution : 100f;

        if (pollution < 50f && benchPoint != null)
        {
            Debug.Log("Customer: Иду к скамейке.");
            _state = State.MoveToBench;
            _target = benchPoint;
        }
        else
        {
            // Обязательно должен быть вызов этого метода!
            CheckBinOrExit();
        }
    }

    private void CheckBinOrExit()
    {
        if (_binExists && _hasFood && binPoint != null)
        {
            _state = State.MoveToBin;
            _target = binPoint;
        }
        else
        {
            _state = State.MoveToRoad;
            _target = roadPoint;
        }
    }

    private void PerformDispose()
    {
        if (_trashBin != null)
        {
            _trashBin.AddTrash();
        }

        RemoveFoodFromHands();
        _hasFood = false;

        _state = State.MoveToRoad;
        _target = roadPoint;
    }

    private void CheckRandomDrop()
    {
        if (_hasFood && Random.value < dropChance)
        {
            DropTrash();
        }
    }

    private void DropTrash()
    {
        if (smallTrashPrefab != null)
        {
            Instantiate(smallTrashPrefab, transform.position, Quaternion.identity);
        }

        RemoveFoodFromHands();
        _hasFood = false;
    }

    private void RemoveFoodFromHands()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void DecideLoopOrExit()
    {
        float pollution = GameManager.Instance != null ? GameManager.Instance.SoilPollution : 100f;
        float roll = Random.value;
        bool shouldLoop = pollution < 50f && roll < 0.5f;

        if (shouldLoop)
        {
            Debug.Log("Customer: Возвращаюсь за добавкой!");
            _state = State.MoveToCashier;
            _target = cashierPoint;
        }
        else
        {
            Debug.Log("Customer: Ухожу.");
            _state = State.Destroy; 
        }
    }

    private IEnumerator WaitThen(System.Action callback)
    {
        Debug.Log($"Customer: Жду {waitTime} секунд...");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Customer: Время вышло!");
        callback?.Invoke();
    }
}