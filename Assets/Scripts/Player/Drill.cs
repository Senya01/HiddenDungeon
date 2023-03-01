using UnityEngine;

public class Drill : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private Animator drillAnimator;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject drill;
    [SerializeField] private Transform drillTip;
    [SerializeField] private Ground ground;
    [SerializeField] private float reloadTime;
    [SerializeField] private int damage;
    
    [HideInInspector] private Quaternion drillRotation;
    [HideInInspector] private bool reloadTimer;
    [HideInInspector] private float reloadTimeLeft;


    private void Start()
    {
        reloadTimeLeft = reloadTime;
    }

    private void Update()
    {
        ProcessDrillRotation();
        ProcessDrillAttack();
        ReloadTimer();
    }
    
    private void FixedUpdate()
    {
        MoveDrill();
    }

    private void ProcessDrillRotation()
    {
        Vector3 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - drill.transform.position;
        float rotationZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        drillRotation = Quaternion.Euler(0, 0, rotationZ);
    }

    private void ProcessDrillAttack()
    {
        if (Input.GetMouseButton(0))
        {
            drillAnimator.SetTrigger("Attack");
            ground.DrillTile(drillTip.position);
        }
    }

    private void ReloadTimer()
    {
        if (reloadTimer)
        {
            if (reloadTimeLeft > 0)
            {
                reloadTimeLeft -= Time.deltaTime;
            }
            else
            {
                reloadTimer = false;
                reloadTimeLeft = reloadTime;
            }
        }
    }

    private void MoveDrill()
    {
        drill.transform.rotation = drillRotation;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && Input.GetMouseButton(0))
        {
            if (!reloadTimer)
            {
                col.GetComponent<Enemy>().Damage(damage);
                reloadTimer = true;
            }
        }
    }
}