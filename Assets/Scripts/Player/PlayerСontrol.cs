using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerСontrol : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private Panel panel;
    [SerializeField] private HpBar hpBar;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private int health;

    [HideInInspector] private Vector2 moveDirection;

    private void Start()
    {
        hpBar.maxValue = health;
    }

    private void Update()
    {
        ProcessMovementInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ProcessMovementInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY);
    }

    private void Move()
    {
        rigidbody.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    public void Damage(int damage)
    {
        health -= damage;
        hpBar.UpdateBar(health);
        if (health <= 0)
        {
            panel.Loss();
        }
    }
}