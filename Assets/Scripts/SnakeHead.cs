using UnityEngine;
using System.Collections.Generic;

public class SnakeHead : MonoBehaviour
{
    public float speed = 5f;
    public GameObject tailSegmentPrefab;
    public int initialLength = 3;
    public List<Transform> tailSegments = new List<Transform>();

    private Vector2 direction = Vector2.right;
    private float moveTimer;
    private float moveInterval = 0.2f;

    void Start()
    {
        for (int i = 0; i < initialLength; i++)
        {
            AddTailSegment();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down)
            direction = Vector2.up;
        if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
            direction = Vector2.down;
        if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
            direction = Vector2.left;
        if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
            direction = Vector2.right;

        moveTimer += Time.deltaTime;
        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;
            Move();
        }
    }

    void Move()
    {
        Vector3 previousPosition = transform.position;
        transform.position += new Vector3(direction.x, direction.y) * speed * moveInterval;

        for (int i = 0; i < tailSegments.Count; i++)
        {
            Vector3 temp = tailSegments[i].position;
            tailSegments[i].position = previousPosition;
            previousPosition = temp;
        }
    }

    void AddTailSegment()
    {
        GameObject segment = Instantiate(tailSegmentPrefab);
        tailSegments.Add(segment.transform);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            AddTailSegment();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Tail"))
        {
            Debug.Log("Game Over");
            // Можно перезапустить игру или перейти на экран Game Over
        }
    }
}
