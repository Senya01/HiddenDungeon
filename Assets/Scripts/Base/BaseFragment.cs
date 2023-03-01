using UnityEngine;

public class BaseFragment : MonoBehaviour, IHpBar
{
    [SerializeField] private HpBar hpBar;
    
    [HideInInspector] private FragmentsController fragmentsController;
    [HideInInspector] private Ground ground;

    private void Start()
    {
        ground = FindObjectOfType<Ground>();
        fragmentsController = FindObjectOfType<FragmentsController>();

        hpBar.maxValue = ground.tilesList[5].health;
    }

    public bool Destroy()
    {
        fragmentsController.AddFragment();
        Destroy(gameObject);
        return true;
    }
    
    public void UpdateHpBar(float hp)
    {
        hpBar.UpdateBar(hp);
    }
}
