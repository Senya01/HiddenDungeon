using UnityEngine;

public class PlayerBase : MonoBehaviour, IHpBar
{
    [SerializeField] private HpBar statusBar;
    
    [HideInInspector] private FragmentsController fragmentsController;
    [HideInInspector] private Ground ground;

    private void Start()
    {
        ground = FindObjectOfType<Ground>();
        fragmentsController = FindObjectOfType<FragmentsController>();

        statusBar.maxValue = ground.tilesList[5].health;
    }

    public void UpdateHpBar(float hp)
    {
        statusBar.UpdateBar(hp);
    }

    public bool Destroy()
    {
        fragmentsController.RemoveFragment();
        return false;
    }
}
