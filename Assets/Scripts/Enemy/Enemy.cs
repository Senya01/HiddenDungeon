using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    private readonly Vector2 Offset = new(0.5f, 0.5f);
    private const float MovementAccuracy = 0.01f;

    [Header("Base settings")]
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private HpBar hpBar;
    [SerializeField] private int health;
    
    [Header("Effects")]
    [SerializeField] private Color explosionColor;
    [SerializeField] private ParticleSystem explosionParticles;

    [Header("Shooting")]
    [SerializeField] private float shootTime;
    [SerializeField] private GameObject bulletPrefab;

    [HideInInspector] private Pathfinding pathfinding;
    [HideInInspector] private Tilemap tilemap;
    [HideInInspector] private Transform player;
    [HideInInspector] private Vector2Int playerPosition;
    [HideInInspector] private List<Vector3> path = new List<Vector3>();
    [HideInInspector] private float shootTimeLeft;

    private void Update()
    {
        Movement();
        ShootTimer();
    }

    private void ShootTimer()
    {
        if (CanShoot())
        {
            if (shootTimeLeft > 0)
            {
                shootTimeLeft -= Time.deltaTime;
            }
            else
            {
                SpawnBullet();
                shootTimeLeft = shootTime;
            }
        }
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerСontrol>().transform;
        pathfinding = FindObjectOfType<Pathfinding>();
        tilemap = pathfinding.tilemap;
        
        shootTimeLeft = shootTime;
        hpBar.maxValue = health;
    }

    private void Movement()
    {
        if (playerPosition != (Vector2Int) tilemap.WorldToCell(player.position))
        {
            playerPosition = (Vector2Int) tilemap.WorldToCell(player.position);
            PathFind();
        }

        if (path.Count > 0)
        {
            Vector2 position = new Vector2(transform.position.x - Offset.x, transform.position.y - Offset.y);
            if (Vector2.Distance(position, path[0]) > MovementAccuracy) MoveTo();
            else path.RemoveAt(0);
        }
        else PathFind();
    }

    private void SpawnBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.direction = player.position - transform.position;
    }

    private void OnDrawGizmos()
    {
        Vector2 prevPos = new Vector2(transform.position.x - Offset.x, transform.position.y - Offset.y);
        foreach (Vector2 pos in path)
        {
            Gizmos.DrawLine(new Vector2(prevPos.x + Offset.x, prevPos.y + Offset.y),
                new Vector2(pos.x + Offset.x, pos.y + Offset.y));
            prevPos = pos;
        }
    }

    private void MoveTo()
    {
        Vector2 target = new Vector2(path[0].x + Offset.x, path[0].y + Offset.y);
        Vector2 vector = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        transform.position = new Vector3(vector.x, vector.y, vector.y);
    }

    private void PathFind()
    {
        Vector2Int enemyPosition = (Vector2Int) tilemap.WorldToCell(transform.position);
        Vector2Int playerPosition = (Vector2Int) tilemap.WorldToCell(player.position);
        path = pathfinding.FindPath(enemyPosition, playerPosition);
    }

    private bool CanShoot()
    {
        RaycastHit2D ray2D = Physics2D.Raycast(transform.position, player.position - transform.position);
        return ray2D.transform == player;
    }

    private void CreateEffect()
    {
        ParticleSystem particles = Instantiate(explosionParticles,
            transform.position, Quaternion.identity);
        particles.startColor = explosionColor;
        particles.Play();
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerСontrol>().Damage(damage);
            CreateEffect();
            Destroy(gameObject);
        }
    }

    public void Damage(int value)
    {
        health -= value;
        hpBar.UpdateBar(health);
        if (health <= 0)
        {
            CreateEffect();
            Destroy(gameObject);
        }
    }
}