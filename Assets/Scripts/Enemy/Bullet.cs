using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float force;
    [SerializeField] private int damage;

    [Header("Effects")]
    [SerializeField] private Color explosionColor;
    [SerializeField] private Color explosionPlayerColor;
    [SerializeField] private ParticleSystem explosionParticles;

    [HideInInspector] public Vector2 direction;

    private void Start()
    {
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Ground") && !col.CompareTag("Drill") && !col.CompareTag("Player")) return;

        ParticleSystem particles = Instantiate(explosionParticles,
            transform.position, Quaternion.identity);
        if (col.CompareTag("Ground") || col.CompareTag("Drill"))
        {
            particles.startColor = explosionColor;
        }
        else if (col.CompareTag("Player"))
        {
            particles.startColor = explosionPlayerColor;
            col.GetComponent<PlayerСontrol>().Damage(damage);
        }

        particles.Play();
        Destroy(gameObject);
    }
}