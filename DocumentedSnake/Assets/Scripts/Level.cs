using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
    \brief Данный класс отвечает за логику первого уровня игры.
*/
public class Level : MonoBehaviour
{
    /// Шаблон для змейки
    [SerializeField] GameObject snakePrefab;
    /// Шаблон для фруктов
    [SerializeField] GameObject fruitPrefab;

    /// Координаты начала экрана
    [SerializeField] Vector2 minPos = Vector2.zero;
    /// Координаты конца экрана
    [SerializeField] Vector2 maxPos = Vector2.one;

    /// Время между движениями змейки
    [SerializeField] float timeBetweenMoves = 0.0f;
    /// Время до следующего движения змейки
    float timeUntilNextMove = 0.0f;

    /// Змейка в виде списка её кусочков
    List<GameObject> snake = new List<GameObject>();
    /// Переменная для блокировки "двойного" поворота
    bool rotationAllowed = true;

    /// Ссылка на текущий фрукт
    GameObject fruit;

    /// Текст на экране для вывода счёта
    TextMeshProUGUI textScore;
    /// Игровой счёт
    int score = 0;

    /** 
        Вызывается на старте игры.
        Служит для инициализации.
    */
    void Start()
    {
        var score = GameObject.FindWithTag("Score");
        textScore = score.GetComponent<TextMeshProUGUI>();
        updateScore();
              
        var head = Instantiate(snakePrefab, transform) as GameObject;
        snake.Add(head);
        head.transform.position = Vector3.zero;
        setRandomRotation(head);

        spawnFruit();
    }

    /** 
        Вызывается на каждый кадр.
        Здесь обрабатывается вся логика.
    */
    void Update()
    {
        int rotationDirection = 0;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rotationDirection += 1;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            rotationDirection -= 1;
        }

        if (rotationDirection != 0 && rotationAllowed)
        {
            rotateSnakeHead(90 * rotationDirection);
            blockRotation();
        }

        decreaseTimeUntilNextMove(Time.deltaTime);
        if (isTimeUntilNextMoveElapsed())
        {
            moveSnake();
            unblockRotation();

            if (canEatFruit())
            {
                eatFruit();
            }

            resetTimeUntilNextMove();
        }
    }

    /** 
        Получить случайное значение поворота
    */
    Vector3 getRandomRotation()
    {
        return new Vector3(0, 0, Random.Range(0, 4) * 90);
    }

    /** 
        Установить объекту случайное значение поворота
    */
    void setRandomRotation(GameObject gameObject)
    {
        gameObject.transform.localEulerAngles = getRandomRotation();
    }

    /** 
        Подвинуть змейку на 1
    */
    void moveSnake()
    {
        if (snake.Count == 0)
        {
            return;
        }

        for (int i = snake.Count - 1; i > 0; --i)
        {
            snake[i].transform.position = snake[i - 1].transform.position;
            snake[i].transform.localEulerAngles = snake[i - 1].transform.localEulerAngles;
        }

        var headTransform = snake[0].transform;
        var headPosition = headTransform.position;
        headPosition += headTransform.up;
        headTransform.position = headPosition;
    }

    /** 
        Поворот головы змейки на нужный угол (-90 или 90)
    */
    void rotateSnakeHead(int angle)
    {
        if (snake.Count == 0 || angle == 0)
        {
            return;
        }

        var headTransform = snake[0].transform;
        var headRotation = headTransform.localEulerAngles;
        headRotation.z += angle;
        headTransform.localEulerAngles = headRotation;
    }

    /** 
        Увеличение длины змейки на 1
    */
    void increaseSnakeLength()
    {
        if (snake.Count == 0)
        {
            return;
        }
        snake.Add(Instantiate(snake[^1], transform));
    }

    /** 
        Проверка, можно ли съесть фрукт
    */
    bool canEatFruit()
    {
        if (snake.Count == 0 || fruit == null)
        {
            return false;
        }

        return snake[0].transform.position == fruit.transform.position;
    }

    /** 
        Обработка логики поедания фрукта
    */
    void eatFruit()
    {
        Destroy(fruit);
        fruit = null;

        increaseSnakeLength();
        spawnFruit();

        score += 1;
        updateScore();
    }

    /** 
        Проверка, пришло ли время для следующего движения
    */
    bool isTimeUntilNextMoveElapsed()
    {
        return timeUntilNextMove <= 0.0f;
    }

    /** 
        Сбрасывает время до следующего движения
    */
    void resetTimeUntilNextMove()
    {
        timeUntilNextMove = timeBetweenMoves / 1000;
    }

    /** 
        Уменьшает время до следующего движения
    */
    void decreaseTimeUntilNextMove(float time)
    {
        timeUntilNextMove -= time;
    }

    /** 
        Блокирует поворот
    */
    void blockRotation()
    {
        rotationAllowed = false;
    }

    /** 
        Разблокирует поворот
    */
    void unblockRotation()
    {
        rotationAllowed = true;
    }

    /** 
        Спавнит фрукт в случайном месте
    */
    void spawnFruit()
    {
        if (fruit != null)
        {
            return;
        }

        var fruitPosition = new Vector3((int)Random.Range(minPos.x, maxPos.x), (int)Random.Range(minPos.y, maxPos.y), 0);
        fruit = Instantiate(fruitPrefab, transform);
        fruit.transform.position = fruitPosition;
    }

    /** 
        Обновляет игровой счёт на экране
    */
    void updateScore()
    {
        textScore.text = $"Score: {score}";
    }
}
