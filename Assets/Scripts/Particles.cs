using UnityEngine;

public class Particles : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private ParticleSystem particleSystem;

    public void Update()
    {
        if (!particleSystem.IsAlive())
            Destroy(gameObject);
    }
}